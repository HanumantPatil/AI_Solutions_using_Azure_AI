"""Simple A2A client to call the running agent service."""

import os
from typing import Dict, List, Optional

import httpx


class A2AClient:
    """Client for testing all A2A agent endpoints."""

    def __init__(self, base_url: str = None, timeout: float = 10.0):
        self.base_url = base_url or os.getenv("A2A_BASE_URL", "http://localhost:8000")
        self.timeout = timeout

    def _client(self) -> httpx.Client:
        return httpx.Client(base_url=self.base_url, timeout=self.timeout)

    # -------------------------------------------------------------------------
    # GET /healthz - Health check
    # -------------------------------------------------------------------------
    def health_check(self) -> Dict:
        """Check if the agent service is healthy."""
        with self._client() as client:
            resp = client.get("/healthz")
            resp.raise_for_status()
            return resp.json()

    # -------------------------------------------------------------------------
    # GET /a2a/metadata - Agent metadata
    # -------------------------------------------------------------------------
    def get_metadata(self) -> Dict:
        """Get agent metadata including capabilities."""
        with self._client() as client:
            resp = client.get("/a2a/metadata")
            resp.raise_for_status()
            return resp.json()

    # -------------------------------------------------------------------------
    # POST /a2a/messages - Send messages (persists to session)
    # -------------------------------------------------------------------------
    def send_messages(
        self,
        session_id: str,
        messages: List[Dict],
        instructions: Optional[str] = None,
        context: Optional[Dict] = None,
    ) -> Dict:
        """Send conversation state to the A2A agent and return the response."""
        payload = {
            "session_id": session_id,
            "messages": messages,
        }
        if instructions:
            payload["instructions"] = instructions
        if context:
            payload["context"] = context

        with self._client() as client:
            resp = client.post("/a2a/messages", json=payload)
            resp.raise_for_status()
            return resp.json()

    # -------------------------------------------------------------------------
    # POST /a2a/messages/stream - Stream response
    # -------------------------------------------------------------------------
    def send_messages_stream(
        self,
        session_id: str,
        messages: List[Dict],
    ) -> str:
        """Send messages and receive streamed response."""
        payload = {
            "session_id": session_id,
            "messages": messages,
        }

        with self._client() as client:
            with client.stream("POST", "/a2a/messages/stream", json=payload) as resp:
                resp.raise_for_status()
                chunks = list(resp.iter_text())
                return "".join(chunks)

    # -------------------------------------------------------------------------
    # GET /a2a/sessions/{id} - Get session history
    # -------------------------------------------------------------------------
    def get_session(self, session_id: str) -> Dict:
        """Retrieve conversation history for a session."""
        with self._client() as client:
            resp = client.get(f"/a2a/sessions/{session_id}")
            resp.raise_for_status()
            return resp.json()

    # -------------------------------------------------------------------------
    # DELETE /a2a/sessions/{id} - Delete session
    # -------------------------------------------------------------------------
    def delete_session(self, session_id: str) -> Dict:
        """Delete a session and its history."""
        with self._client() as client:
            resp = client.delete(f"/a2a/sessions/{session_id}")
            resp.raise_for_status()
            return resp.json()

    # -------------------------------------------------------------------------
    # POST /a2a/sessions/cleanup - Remove stale sessions
    # -------------------------------------------------------------------------
    def cleanup_sessions(self) -> Dict:
        """Remove stale sessions older than TTL."""
        with self._client() as client:
            resp = client.post("/a2a/sessions/cleanup")
            resp.raise_for_status()
            return resp.json()


def main() -> None:
    """Run examples for all endpoints."""
    client = A2AClient()
    session_id = "test-session-demo"

    print("=" * 60)
    print("A2A Agent Client - Testing All Endpoints")
    print("=" * 60)

    # 1. Health check
    print("\n1. GET /healthz - Health Check")
    print("-" * 40)
    result = client.health_check()
    print(f"   Response: {result}")

    # 2. Get metadata
    print("\n2. GET /a2a/metadata - Agent Metadata")
    print("-" * 40)
    result = client.get_metadata()
    print(f"   Name: {result.get('name')}")
    print(f"   Protocol: {result.get('protocol')}")
    print(f"   Capabilities: {result.get('capabilities')}")

    # 3. Send messages
    print("\n3. POST /a2a/messages - Send Messages")
    print("-" * 40)
    messages = [
        {"role": "system", "content": [{"type": "text", "text": "You are concise."}]},
        {"role": "user", "content": [{"type": "text", "text": "Hello agent!"}]},
    ]
    result = client.send_messages(
        session_id=session_id,
        messages=messages,
        instructions="Keep responses brief",
        context={"user_id": "demo-user"},
    )
    print(f"   Assistant: {result['messages'][0]['content'][0]['text']}")
    print(f"   Turn count: {result['metadata'].get('session_turn_count')}")

    # 4. Send another message (tests session persistence)
    print("\n4. POST /a2a/messages - Second Turn (Session Persistence)")
    print("-" * 40)
    messages2 = [
        {"role": "user", "content": [{"type": "text", "text": "What did I say before?"}]},
    ]
    result = client.send_messages(session_id=session_id, messages=messages2)
    print(f"   Assistant: {result['messages'][0]['content'][0]['text']}")
    print(f"   Turn count: {result['metadata'].get('session_turn_count')}")

    # 5. Stream messages
    print("\n5. POST /a2a/messages/stream - Stream Response")
    print("-" * 40)
    stream_messages = [
        {"role": "user", "content": [{"type": "text", "text": "Stream this response"}]},
    ]
    streamed = client.send_messages_stream(session_id="stream-session", messages=stream_messages)
    print(f"   Streamed response: {streamed}")

    # 6. Get session history
    print("\n6. GET /a2a/sessions/{id} - Get Session History")
    print("-" * 40)
    result = client.get_session(session_id)
    print(f"   Session ID: {result.get('session_id')}")
    print(f"   Message count: {result.get('message_count')}")
    print(f"   Last access: {result.get('last_access')}")

    # 7. Cleanup stale sessions
    print("\n7. POST /a2a/sessions/cleanup - Cleanup Stale Sessions")
    print("-" * 40)
    result = client.cleanup_sessions()
    print(f"   Removed sessions: {result.get('removed_sessions')}")

    # 8. Delete session
    print("\n8. DELETE /a2a/sessions/{id} - Delete Session")
    print("-" * 40)
    result = client.delete_session(session_id)
    print(f"   Status: {result.get('status')}")
    print(f"   Deleted session: {result.get('session_id')}")

    print("\n" + "=" * 60)
    print("All endpoint tests completed!")
    print("=" * 60)


if __name__ == "__main__":
    main()
