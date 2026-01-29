"""Simple client to probe the A2A SDK server.

This script exercises the available HTTP endpoints exposed by `server.py`:
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

        print("✓ All checks completed")
    except httpx.HTTPError as exc:
        print(f"Request failed: {exc}")
        print("Is the server running? Start it with `python server.py`.")
        sys.exit(1)


if __name__ == "__main__":
    main()
