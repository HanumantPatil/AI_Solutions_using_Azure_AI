from __future__ import annotations

import asyncio
import logging
import os
import threading
from abc import ABC, abstractmethod
from collections import defaultdict
from datetime import datetime, timedelta
from typing import Any, Dict, List, Literal, Optional

from fastapi import FastAPI, HTTPException, Request
from fastapi.responses import StreamingResponse
from pydantic import BaseModel, Field, validator

# ---------------------------------------------------------------------------
# Logging
# ---------------------------------------------------------------------------
logging.basicConfig(level=logging.INFO)
logger = logging.getLogger(__name__)

# ---------------------------------------------------------------------------
# Session store abstraction & implementations
# ---------------------------------------------------------------------------
SESSION_TTL_MINUTES = int(os.getenv("A2A_SESSION_TTL_MINUTES", "60"))


class SessionStore(ABC):
    """Abstract base class for session storage."""

    @abstractmethod
    async def get(self, session_id: str) -> Dict[str, Any]:
        """Return session dict (create if missing)."""

    @abstractmethod
    async def save(self, session_id: str, data: Dict[str, Any]) -> None:
        """Persist session dict."""

    @abstractmethod
    async def delete(self, session_id: str) -> bool:
        """Delete session. Return True if existed."""

    @abstractmethod
    async def cleanup_stale(self) -> int:
        """Remove sessions older than TTL. Return count removed."""


class InMemorySessionStore(SessionStore):
    """Thread-safe in-memory session store with TTL support."""

    def __init__(self) -> None:
        self._lock = threading.Lock()
        self._store: Dict[str, Dict[str, Any]] = defaultdict(
            lambda: {"messages": [], "context": None, "last_access": datetime.utcnow()}
        )

    async def get(self, session_id: str) -> Dict[str, Any]:
        with self._lock:
            session = self._store[session_id]
            session["last_access"] = datetime.utcnow()
            return session

    async def save(self, session_id: str, data: Dict[str, Any]) -> None:
        with self._lock:
            self._store[session_id] = data

    async def delete(self, session_id: str) -> bool:
        with self._lock:
            if session_id in self._store:
                del self._store[session_id]
                return True
            return False

    async def cleanup_stale(self) -> int:
        cutoff = datetime.utcnow() - timedelta(minutes=SESSION_TTL_MINUTES)
        removed = 0
        with self._lock:
            stale = [sid for sid, s in self._store.items() if s["last_access"] < cutoff]
            for sid in stale:
                del self._store[sid]
                removed += 1
        return removed


# ---------------------------------------------------------------------------
# Optional Redis session store (requires redis package)
# ---------------------------------------------------------------------------
REDIS_AVAILABLE = False
try:
    import redis.asyncio as aioredis

    REDIS_AVAILABLE = True
except ImportError:
    aioredis = None  # type: ignore


class RedisSessionStore(SessionStore):
    """Redis-backed session store for horizontal scaling."""

    def __init__(self, client: Any, prefix: str = "a2a:session:") -> None:
        self._client = client
        self._prefix = prefix

    def _key(self, session_id: str) -> str:
        return f"{self._prefix}{session_id}"

    async def get(self, session_id: str) -> Dict[str, Any]:
        import json

        raw = await self._client.get(self._key(session_id))
        if raw:
            data = json.loads(raw)
            data["last_access"] = datetime.utcnow().isoformat()
            await self.save(session_id, data)
            return data
        return {"messages": [], "context": None, "last_access": datetime.utcnow().isoformat()}

    async def save(self, session_id: str, data: Dict[str, Any]) -> None:
        import json

        await self._client.setex(
            self._key(session_id),
            SESSION_TTL_MINUTES * 60,
            json.dumps(data, default=str),
        )

    async def delete(self, session_id: str) -> bool:
        result = await self._client.delete(self._key(session_id))
        return result > 0

    async def cleanup_stale(self) -> int:
        # Redis handles TTL automatically via SETEX
        return 0


# ---------------------------------------------------------------------------
# Store initialization
# ---------------------------------------------------------------------------
use_redis = os.getenv("USE_REDIS", "false").lower() in ("true", "1", "yes")
session_store: SessionStore

if use_redis and REDIS_AVAILABLE:
    logger.info("Initializing Redis-based session store...")
    redis_host = os.getenv("REDIS_HOST", "localhost")
    redis_port = int(os.getenv("REDIS_PORT", "6379"))
    redis_password = os.getenv("REDIS_PASSWORD")

    _redis_client = aioredis.Redis(
        host=redis_host,
        port=redis_port,
        password=redis_password,
        decode_responses=True,
    )
    session_store = RedisSessionStore(_redis_client, prefix="a2a:session:")
    logger.info("✓ Redis session store initialized")
else:
    if use_redis and not REDIS_AVAILABLE:
        logger.warning(
            "Redis requested but redis package not available. Using in-memory storage."
        )
    else:
        logger.info("Using in-memory session store (no Redis)")
    session_store = InMemorySessionStore()


app = FastAPI(
    title="A2A Agent Service",
    version="0.1.0",
    description="Minimal A2A Agent Protocol service implemented in Python with FastAPI.",
)


class Content(BaseModel):
    """Represents a single content block in an A2A message."""

    type: Literal["text"] = Field(
        ..., description="Type of content block. This sample only supports text content."
    )
    text: str = Field(..., description="UTF-8 text content.")


class Message(BaseModel):
    """Represents an A2A message exchanged between agents."""

    role: Literal["user", "assistant", "system"] = Field(
        ..., description="Message author role."
    )
    content: List[Content] = Field(..., description="Ordered list of content blocks.")

    @validator("content")
    def ensure_non_empty(cls, value: List[Content]) -> List[Content]:
        if not value:
            raise ValueError("content must contain at least one block")
        return value


class A2ARequest(BaseModel):
    """Envelope received from an upstream agent."""

    session_id: str = Field(..., description="Conversation/session identifier.")
    messages: List[Message] = Field(
        ..., description="Conversation history in order, newest last."
    )
    context: Optional[Dict[str, object]] = Field(
        default=None,
        description="Optional shared state passed across calls (kept opaque).",
    )
    instructions: Optional[str] = Field(
        default=None, description="Optional additional system-level guidance."
    )

    @validator("messages")
    def ensure_messages_present(cls, value: List[Message]) -> List[Message]:
        if not value:
            raise ValueError("messages must contain at least one entry")
        return value


class Metadata(BaseModel):
    name: str
    description: str
    version: str
    protocol: str
    capabilities: Dict[str, object]


class A2AResponse(BaseModel):
    messages: List[Message]
    metadata: Dict[str, object]


def generate_reply(
    full_history: List[Message], instructions: Optional[str], context: Optional[Dict]
) -> str:
    """Return a lightweight response based on the full conversation history."""

    last_user = next((m for m in reversed(full_history) if m.role == "user"), None)
    if last_user is None:
        raise HTTPException(status_code=400, detail="At least one user message is required")

    user_text = " ".join(block.text for block in last_user.content if block.type == "text")
    instructions_text = instructions or "none"
    context_hint = "context provided" if context else "no context provided"
    turn_count = sum(1 for m in full_history if m.role == "user")

    reply = (
        "I am a sample A2A agent. "
        f"You said: '{user_text}'. "
        f"Instructions: {instructions_text}. "
        f"Context: {context_hint}. "
        f"This is turn {turn_count} in this session."
    )
    return reply


@app.get("/a2a/metadata", response_model=Metadata)
async def get_metadata() -> Metadata:
    """Return static metadata describing this agent service."""

    return Metadata(
        name="Sample Python A2A Agent",
        description="Reference implementation of the A2A Agent Protocol using FastAPI.",
        version="0.1.0",
        protocol="a2a-agent-protocol-v1",
        capabilities={
            "supports_streaming": True,
            "supported_languages": ["en"],
            "content_types": ["text"],
        },
    )


@app.post("/a2a/messages", response_model=A2AResponse)
async def post_messages(payload: A2ARequest, request: Request) -> A2AResponse:
    """Handle an A2A message batch and return the assistant's reply."""

    # Retrieve or create session
    session = await session_store.get(payload.session_id)

    # Rebuild messages from session (handle both dict and Message types)
    stored_msgs = session.get("messages", [])
    history: List[Message] = []
    for m in stored_msgs:
        if isinstance(m, Message):
            history.append(m)
        elif isinstance(m, dict):
            history.append(Message(**m))

    # Merge incoming messages into session history
    history.extend(payload.messages)
    if payload.context is not None:
        session["context"] = payload.context

    # Generate reply using full history
    reply_text = generate_reply(history, payload.instructions, session.get("context"))

    response_message = Message(
        role="assistant",
        content=[Content(type="text", text=reply_text)],
    )

    # Store assistant reply in session
    history.append(response_message)
    session["messages"] = [m.model_dump() for m in history]
    await session_store.save(payload.session_id, session)

    return A2AResponse(
        messages=[response_message],
        metadata={
            "received_at": datetime.utcnow().isoformat() + "Z",
            "request_ip": request.client.host if request.client else None,
            "protocol": "a2a-agent-protocol-v1",
            "session_turn_count": sum(1 for m in history if m.role == "user"),
        },
    )


@app.get("/healthz")
async def healthcheck() -> Dict[str, str]:
    return {"status": "ok"}


@app.post("/a2a/messages/stream")
async def post_messages_stream(payload: A2ARequest, request: Request) -> StreamingResponse:
    """Stream the assistant reply in chunks to demonstrate streaming support."""

    session = await session_store.get(payload.session_id)

    stored_msgs = session.get("messages", [])
    history: List[Message] = []
    for m in stored_msgs:
        if isinstance(m, Message):
            history.append(m)
        elif isinstance(m, dict):
            history.append(Message(**m))

    history.extend(payload.messages)
    if payload.context is not None:
        session["context"] = payload.context

    reply_text = generate_reply(history, payload.instructions, session.get("context"))

    response_message = Message(
        role="assistant",
        content=[Content(type="text", text=reply_text)],
    )
    history.append(response_message)
    session["messages"] = [m.model_dump() for m in history]
    await session_store.save(payload.session_id, session)

    midpoint = max(1, len(reply_text) // 2)

    async def streamer():
        yield reply_text[:midpoint].encode("utf-8")
        yield reply_text[midpoint:].encode("utf-8")

    metadata = {
        "received_at": datetime.utcnow().isoformat() + "Z",
        "request_ip": request.client.host if request.client else None,
        "protocol": "a2a-agent-protocol-v1",
        "stream": True,
    }

    headers = {"X-A2A-Metadata": str(metadata)}
    return StreamingResponse(streamer(), media_type="text/plain", headers=headers)


# ---------------------------------------------------------------------------
# Session management endpoints
# ---------------------------------------------------------------------------
@app.get("/a2a/sessions/{session_id}")
async def get_session_history(session_id: str) -> Dict:
    """Retrieve conversation history for a session."""
    session = await session_store.get(session_id)
    return {
        "session_id": session_id,
        "message_count": len(session.get("messages", [])),
        "messages": session.get("messages", []),
        "context": session.get("context"),
        "last_access": session.get("last_access"),
    }


@app.delete("/a2a/sessions/{session_id}")
async def delete_session(session_id: str) -> Dict[str, str]:
    """Delete a session and its history."""
    deleted = await session_store.delete(session_id)
    if deleted:
        return {"status": "deleted", "session_id": session_id}
    raise HTTPException(status_code=404, detail="Session not found")


@app.post("/a2a/sessions/cleanup")
async def cleanup_sessions() -> Dict[str, int]:
    """Remove stale sessions older than TTL."""
    removed = await session_store.cleanup_stale()
    return {"removed_sessions": removed}


if __name__ == "__main__":
    import uvicorn

    max_concurrency = int(os.getenv("A2A_MAX_CONCURRENCY", "1000"))
    backlog = int(os.getenv("A2A_BACKLOG", "2048"))

    uvicorn.run(
        "main:app",
        host="0.0.0.0",
        port=8000,
        reload=True,
        limit_concurrency=max_concurrency,
        backlog=backlog,
    )
