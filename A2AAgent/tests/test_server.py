import pathlib
import sys

import pytest
from starlette.testclient import TestClient

sys.path.append(str(pathlib.Path(__file__).resolve().parents[1]))

from server import create_server_app


def _client() -> TestClient:
    app = create_server_app(host="127.0.0.1", port=8000)
    return TestClient(app)


def test_health_endpoint_returns_ok():
    with _client() as client:
        resp = client.get("/health")
        assert resp.status_code == 200
        body = resp.json()
        assert body.get("status") == "ok"
        assert "timestamp" in body


def test_root_endpoint_returns_service_info():
    with _client() as client:
        resp = client.get("/")
        assert resp.status_code == 200
        body = resp.json()
        assert body.get("service") == "A2A Agent Service"
        assert body.get("protocol") == "a2a-agent-protocol-v1"
        assert "/.well-known/agent.json" in body.get("endpoints", {}).values()


def test_agent_card_is_available():
    with _client() as client:
        resp = client.get("/.well-known/agent-card.json")
        assert resp.status_code == 200
        card = resp.json()
        assert card.get("name") == "Sample Python A2A Agent"
        assert "skills" in card
        assert card.get("capabilities", {}).get("streaming") is not None
        assert card.get("protocolVersion")
