import os

import httpx
import pytest

BASE_URL = os.getenv("A2A_BASE_URL")


@pytest.mark.integration
@pytest.mark.skipif(
    not BASE_URL,
    reason="Set A2A_BASE_URL (e.g., http://localhost:8000) to run live agent tests.",
)
def test_live_metadata_and_reply():
    # Assumes the agent service is already running at BASE_URL.
    with httpx.Client(base_url=BASE_URL, timeout=5.0) as client:
        meta = client.get("/a2a/metadata")
        assert meta.status_code == 200
        assert meta.json().get("protocol") == "a2a-agent-protocol-v1"

        payload = {
            "session_id": "live-session",
            "messages": [
                {"role": "user", "content": [{"type": "text", "text": "ping"}]}
            ],
        }
        resp = client.post("/a2a/messages", json=payload)
        assert resp.status_code == 200
        data = resp.json()
        assert data["messages"][0]["role"] == "assistant"
        assert "ping" in data["messages"][0]["content"][0]["text"]
