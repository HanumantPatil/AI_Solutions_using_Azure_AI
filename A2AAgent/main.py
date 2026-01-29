"""A2A Agent Server using the official a2a-sdk package.

This file now contains the full Starlette-based implementation (previously in
``server.py``) and serves as the single entrypoint: run it with ``python main.py``.
"""

from __future__ import annotations

import asyncio
import logging
import os
from datetime import datetime
from typing import Any, AsyncIterable, Dict, List, Optional

import httpx
import uvicorn
from starlette.applications import Starlette
from starlette.middleware.cors import CORSMiddleware
from starlette.requests import Request
from starlette.responses import JSONResponse, Response
from starlette.routing import Route

# ---------------------------------------------------------------------------
# Logging
# ---------------------------------------------------------------------------
logging.basicConfig(
    level=logging.INFO,
    format="%(asctime)s - %(name)s - %(levelname)s - %(message)s",
)
logger = logging.getLogger(__name__)

# ---------------------------------------------------------------------------
# A2A SDK imports (with fallback for optional Redis components)
# ---------------------------------------------------------------------------
try:
    from a2a.server.apps import A2AStarletteApplication
    from a2a.server.request_handlers import DefaultRequestHandler
    from a2a.server.tasks import InMemoryTaskStore
    from a2a.server.agent_execution import AgentExecutor
    from a2a.types import (
        AgentCard,
        AgentCapabilities,
        AgentSkill,
        Message,
        Part,
        Task,
        TaskState,
        TaskStatus,
        TaskStatusUpdateEvent,
        TextPart,
    )
    from a2a.server.events import EventQueue
    from a2a.utils import new_agent_text_message
    from sample_agent_executor import SampleAgentExecutor

    A2A_SDK_AVAILABLE = True
except ImportError as e:
    logger.warning(f"a2a-sdk not available: {e}. Install with: pip install a2a-sdk")
    A2A_SDK_AVAILABLE = False

# Optional Redis components
REDIS_AVAILABLE = False
try:
    import redis.asyncio as aioredis
    from a2a.server.tasks import RedisTaskStore
    from a2a.server.queues import RedisListQueueManager
    from a2a.server.push_notification_config import (
        InMemoryPushNotificationConfigStore,
        RedisPushNotificationConfigStore,
    )

    REDIS_AVAILABLE = True
except ImportError:
    aioredis = None  # type: ignore
    try:
        from a2a.server.push_notification_config import InMemoryPushNotificationConfigStore
    except ImportError:
        InMemoryPushNotificationConfigStore = None  # type: ignore

# Fallback in-memory push notification store when optional dependency is unavailable
if InMemoryPushNotificationConfigStore is None:  # type: ignore

    class InMemoryPushNotificationConfigStore:  # type: ignore
        def __init__(self) -> None:
            self._store: Dict[str, Dict[str, Any]] = {}

        async def get(self, key: str) -> Optional[Dict[str, Any]]:
            return self._store.get(key)

        async def set(self, key: str, value: Dict[str, Any]) -> None:
            self._store[key] = value

        async def delete(self, key: str) -> None:
            self._store.pop(key, None)

# ---------------------------------------------------------------------------
# Configuration
# ---------------------------------------------------------------------------
USE_REDIS = os.getenv("USE_REDIS", "false").lower() in ("true", "1", "yes")
REDIS_HOST = os.getenv("REDIS_HOST", "localhost")
REDIS_PORT = int(os.getenv("REDIS_PORT", "6379"))
REDIS_PASSWORD = os.getenv("REDIS_PASSWORD")
SERVER_HOST = os.getenv("A2A_HOST", "0.0.0.0")
SERVER_PORT = int(os.getenv("A2A_PORT", "8000"))


# ---------------------------------------------------------------------------
# Health check and root handlers
# ---------------------------------------------------------------------------
async def health_check(request: Request) -> JSONResponse:
    """Health check endpoint."""
    return JSONResponse({"status": "ok", "timestamp": datetime.utcnow().isoformat()})


async def root_handler(request: Request) -> JSONResponse:
    """Root endpoint with service info."""
    return JSONResponse(
        {
            "service": "A2A Agent Service",
            "version": "0.2.0",
            "protocol": "a2a-agent-protocol-v1",
            "endpoints": {
                "agent_card": "/.well-known/agent.json",
                "health": "/health",
            },
        }
    )


# ---------------------------------------------------------------------------
# Server factory
# ---------------------------------------------------------------------------
def create_server_app(host: str = SERVER_HOST, port: int = SERVER_PORT) -> Starlette:
    """Create and configure the A2A server application."""
    logger.info("=" * 80)
    logger.info("Initializing A2A Agent Server")
    logger.info("=" * 80)

    if not A2A_SDK_AVAILABLE:
        raise RuntimeError(
            "a2a-sdk is required. Install with: pip install a2a-sdk"
        )

    # Create agent card (service metadata)
    logger.info("Creating agent card...")
    agent_card = AgentCard(
        name="Sample Python A2A Agent",
        description="Reference implementation of the A2A Agent Protocol using the official SDK.",
        url=f"http://{host}:{port}",
        version="0.2.0",
        defaultInputModes=["text"],
        defaultOutputModes=["text"],
        capabilities=AgentCapabilities(
            streaming=True,
            pushNotifications=USE_REDIS and REDIS_AVAILABLE,
        ),
        skills=[
            AgentSkill(
                id="chat",
                name="Chat",
                description="General conversational assistant",
                tags=["chat", "conversation"],
                examples=["Hello!", "What can you do?"],
            )
        ],
    )

    # Create HTTP client for outbound requests
    httpx_client = httpx.AsyncClient(timeout=30.0)

    # Initialize storage components
    task_store: Any
    queue_manager: Any
    push_config_store: Any

    if USE_REDIS and REDIS_AVAILABLE:
        logger.info("Initializing Redis-based components...")

        # Create push notification components
        push_config_store = InMemoryPushNotificationConfigStore()

        # Create Redis client
        redis_client = aioredis.Redis(
            host=REDIS_HOST,
            port=REDIS_PORT,
            password=REDIS_PASSWORD,
            decode_responses=True,
        )

        # Initialize Redis-backed components
        task_store = RedisTaskStore(redis_client, prefix="a2a:tasks:")
        queue_manager = RedisListQueueManager(redis_client, prefix="a2a:queues:")
        push_config_store = RedisPushNotificationConfigStore(
            redis_client, prefix="a2a:push:"
        )

        logger.info("✓ Redis components initialized")
    else:
        if USE_REDIS and not REDIS_AVAILABLE:
            logger.warning(
                "Redis requested but redis/a2a_redis packages not available. "
                "Using in-memory storage."
            )
        else:
            logger.info("Using in-memory components (no Redis)")

        # Use in-memory components
        task_store = InMemoryTaskStore()
        queue_manager = None  # Will use default
        push_config_store = InMemoryPushNotificationConfigStore()

    # Create agent executor
    logger.info("Creating agent executor...")
    executor = SampleAgentExecutor()

    # Create A2A request handler
    logger.info("Creating A2A request handler...")
    request_handler = DefaultRequestHandler(
        agent_executor=executor,
        task_store=task_store,
    )

    # Create A2A Starlette application
    logger.info("Building A2A Starlette application...")
    server = A2AStarletteApplication(
        agent_card=agent_card,
        http_handler=request_handler,
    )
    app = server.build()

    # Add CORS middleware for cross-origin requests
    app.add_middleware(
        CORSMiddleware,
        allow_origins=["*"],
        allow_methods=["*"],
        allow_headers=["*"],
    )

    # Add custom routes
    app.router.routes.append(Route("/health", health_check, methods=["GET"]))
    app.router.routes.append(Route("/", root_handler, methods=["GET"]))

    logger.info("=" * 80)
    logger.info("✓ Server initialized successfully")
    logger.info("=" * 80)

    return app


def run_server(host: str = SERVER_HOST, port: int = SERVER_PORT) -> None:
    """Run the A2A Agent server."""
    app = create_server_app(host, port)

    logger.info("Starting server on %s:%s...", host, port)
    logger.info(
        "Agent Card available at: http://%s:%s/.well-known/agent.json", host, port
    )

    try:
        uvicorn.run(
            app,
            host=host,
            port=port,
            workers=1,  # Single worker for async operations
            limit_concurrency=1000,  # Max concurrent requests
            limit_max_requests=2000,  # Restart worker after N requests
            timeout_keep_alive=60,  # Keep connections alive
            log_level="info",
        )
    except Exception as e:
        logger.error("Server failed to start: %s", e, exc_info=True)


def main() -> None:
    """Start the A2A SDK server (script entrypoint)."""
    run_server()


if __name__ == "__main__":
    main()
