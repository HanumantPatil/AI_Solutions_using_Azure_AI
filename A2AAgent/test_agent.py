"""Simple A2A client to call the running agent service."""

import os
from typing import Dict, List

import httpx


def send_messages(base_url: str, session_id: str, messages: List[Dict]) -> Dict:
	"""Send conversation state to the A2A agent and return the response JSON."""

	payload = {
		"session_id": session_id,
		"messages": messages,
	}

	with httpx.Client(base_url=base_url, timeout=10.0) as client:
		resp = client.post("/a2a/messages", json=payload)
		resp.raise_for_status()
		return resp.json()


def main() -> None:
	base_url = os.getenv("A2A_BASE_URL", "http://localhost:8000")
	messages = [
		{"role": "system", "content": [{"type": "text", "text": "You are concise."}]},
		{"role": "user", "content": [{"type": "text", "text": "Hello agent"}]},
	]

	result = send_messages(base_url, "cli-session", messages)
	print("Response:\n", result)


if __name__ == "__main__":
	main()
