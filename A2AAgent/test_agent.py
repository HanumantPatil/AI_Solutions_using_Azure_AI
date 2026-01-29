"""Simple client to probe the A2A SDK server.

This script exercises the available HTTP endpoints exposed by `main.py`:
- GET /health
- GET /
- GET /.well-known/agent-card.json (preferred) and agent.json (legacy)

Usage:
    set A2A_BASE_URL=http://localhost:8000
    python test_agent.py
"""

import os
import sys
from typing import Dict, Tuple

import httpx


BASE_URL = os.getenv("A2A_BASE_URL", "http://localhost:8000")
RPC_PATH = os.getenv("A2A_RPC_PATH", "/")


def _client() -> httpx.Client:
    return httpx.Client(base_url=BASE_URL, timeout=10.0)


def check_health() -> Dict:
    with _client() as client:
        resp = client.get("/health")
        resp.raise_for_status()
        return resp.json()


def get_root() -> Dict:
    with _client() as client:
        resp = client.get("/")
        resp.raise_for_status()
        return resp.json()


def get_agent_card() -> Tuple[Dict, str]:
    """Fetch the agent card, preferring the new endpoint but falling back if needed."""

    with _client() as client:
        resp = client.get("/.well-known/agent-card.json")
        if resp.status_code == 404:
            resp = client.get("/.well-known/agent.json")
        resp.raise_for_status()
        endpoint = str(resp.url)
        return resp.json(), endpoint


def send_chat_message(message: str = "Hello agent!", timeout: float = 8.0) -> Dict:
    """Send a chat message via A2A JSON-RPC and return the final task payload."""

    req_id = "send-1"
    msg_id = "msg-1"

    payload = {
        "jsonrpc": "2.0",
        "id": req_id,
        "method": "message/send",
        "params": {
            "message": {
                "message_id": msg_id,
                "role": "user",
                "parts": [{"kind": "text", "text": message}],
            }
        },
    }

    with _client() as client:
        create_resp = client.post(RPC_PATH, json=payload)
        create_resp.raise_for_status()
        created = create_resp.json()
        result = created.get("result", {})
        task_id = result.get("id")
        if not task_id:
            raise RuntimeError(f"Unexpected message/send response: {created}")

        # Poll task status briefly
        poll_payload = {
            "jsonrpc": "2.0",
            "id": "poll-1",
            "method": "tasks/get",
            "params": {"id": task_id},
        }

        elapsed = 0.0
        step = 0.5
        while elapsed <= timeout:
            poll_resp = client.post(RPC_PATH, json=poll_payload)
            poll_resp.raise_for_status()
            task = poll_resp.json().get("result", {})
            status = task.get("status", {})
            state = status.get("state") if isinstance(status, dict) else status
            if state in {"succeeded", "failed", "completed", "canceled"}:
                return task
            elapsed += step
        return task  # best effort


def main() -> None:
    print("A2A SDK server probe")
    print(f"BASE_URL: {BASE_URL}")

    try:
        health = check_health()
        print("- /health ->", health)

        root = get_root()
        print("- / ->", root)

        card, endpoint = get_agent_card()
        print(
            f"- Agent card ({endpoint}) -> name={card.get('name')} protocol={card.get('protocolVersion')}"
        )

        task = send_chat_message("Hello from test_agent.py")
        print(f"- createTask -> status={task.get('status')} output={task.get('output')}")

        print("✓ All checks completed")
    except httpx.HTTPError as exc:
        print(f"Request failed: {exc}")
        print("Is the server running? Start it with `python main.py`.")
        sys.exit(1)


if __name__ == "__main__":
    main()
