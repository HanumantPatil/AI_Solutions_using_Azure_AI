import os

import httpx
import pytest

BASE_URL = os.getenv("A2A_BASE_URL")


@pytest.mark.integration
@pytest.mark.skipif(
    not BASE_URL,
    reason="Set A2A_BASE_URL (e.g., http://localhost:8000) to run live agent tests.",
)
def test_live_agent_card_and_health():
    """Validate the running SDK server exposes the agent card and health endpoint."""

    with httpx.Client(base_url=BASE_URL, timeout=5.0) as client:
        card = client.get("/.well-known/agent.json")
        assert card.status_code == 200
        card_data = card.json()
        assert card_data.get("name") == "Sample Python A2A Agent"

        health = client.get("/health")
        assert health.status_code == 200
        assert health.json().get("status") == "ok"
