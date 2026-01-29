import pytest
from fastapi.testclient import TestClient

from main import app

client = TestClient(app)


def test_healthcheck():
    resp = client.get("/healthz")
    assert resp.status_code == 200
    assert resp.json() == {"status": "ok"}


def test_metadata():
    resp = client.get("/a2a/metadata")
    assert resp.status_code == 200
    body = resp.json()
    assert body["protocol"] == "a2a-agent-protocol-v1"
    assert "content_types" in body["capabilities"]


def test_post_messages_returns_reply():
    payload = {
        "session_id": "test-session",
        "messages": [
            {"role": "user", "content": [{"type": "text", "text": "Hello"}]}
        ],
    }
    resp = client.post("/a2a/messages", json=payload)
    assert resp.status_code == 200
    body = resp.json()
    assert body["messages"][0]["role"] == "assistant"
    assert "Hello" in body["messages"][0]["content"][0]["text"]


def test_post_messages_requires_user_message():
    payload = {
        "session_id": "test-session",
        "messages": [
            {"role": "assistant", "content": [{"type": "text", "text": "Hi"}]}
        ],
    }
    resp = client.post("/a2a/messages", json=payload)
    assert resp.status_code == 400
    assert "user message" in resp.json()["detail"]


def test_post_messages_rejects_empty_content():
    payload = {
        "session_id": "test-session",
        "messages": [
            {"role": "user", "content": []}
        ],
    }
    resp = client.post("/a2a/messages", json=payload)
    assert resp.status_code == 422


def test_post_messages_echoes_instructions_and_context_hint():
    payload = {
        "session_id": "test-session",
        "instructions": "keep it short",
        "context": {"foo": "bar"},
        "messages": [
            {"role": "user", "content": [{"type": "text", "text": "Hello"}]}
        ],
    }
    resp = client.post("/a2a/messages", json=payload)
    assert resp.status_code == 200
    text = resp.json()["messages"][0]["content"][0]["text"]
    assert "keep it short" in text
    assert "context provided" in text


def test_post_messages_stream_returns_chunks():
    payload = {
        "session_id": "test-session",
        "messages": [
            {"role": "user", "content": [{"type": "text", "text": "stream me"}]}
        ],
    }

    with client.stream("POST", "/a2a/messages/stream", json=payload) as resp:
        assert resp.status_code == 200
        chunks = list(resp.iter_text())

    body = "".join(chunks)
    assert "stream me" in body
