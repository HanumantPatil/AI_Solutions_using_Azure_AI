from __future__ import annotations

from datetime import datetime
from typing import Dict, List, Literal, Optional

from fastapi import FastAPI, HTTPException, Request
from fastapi.responses import StreamingResponse
from pydantic import BaseModel, Field, validator

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


def generate_reply(request: A2ARequest) -> str:
    """Return a lightweight response based on the latest user message."""

    last_user = next((m for m in reversed(request.messages) if m.role == "user"), None)
    if last_user is None:
        raise HTTPException(status_code=400, detail="At least one user message is required")

    user_text = " ".join(block.text for block in last_user.content if block.type == "text")
    instructions = request.instructions or ""
    context_hint = "context provided" if request.context else "no context provided"

    reply = (
        "I am a sample A2A agent. "
        f"You said: '{user_text}'. "
        f"Instructions: {instructions or 'none'}. "
        f"Context: {context_hint}."
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

    reply_text = generate_reply(payload)

    response_message = Message(
        role="assistant",
        content=[Content(type="text", text=reply_text)],
    )

    return A2AResponse(
        messages=[response_message],
        metadata={
            "received_at": datetime.utcnow().isoformat() + "Z",
            "request_ip": request.client.host if request.client else None,
            "protocol": "a2a-agent-protocol-v1",
        },
    )


@app.get("/healthz")
async def healthcheck() -> Dict[str, str]:
    return {"status": "ok"}


@app.post("/a2a/messages/stream")
async def post_messages_stream(payload: A2ARequest, request: Request) -> StreamingResponse:
    """Stream the assistant reply in chunks to demonstrate streaming support."""

    reply_text = generate_reply(payload)
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


if __name__ == "__main__":
    import uvicorn

    uvicorn.run("main:app", host="0.0.0.0", port=8000, reload=True)
