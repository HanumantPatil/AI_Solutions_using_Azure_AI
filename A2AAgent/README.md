# Python A2A Agent Protocol Service

A minimal FastAPI service that speaks the A2A (agent-to-agent) protocol. It exposes metadata and message handling endpoints so another agent or orchestrator can call into this service. The implementation is intentionally lightweight and self-contained.

## Quickstart

1. Install dependencies (Python 3.10+):
   ```bash
   python -m venv .venv
   .venv\\Scripts\\activate
   pip install -r requirements.txt
   ```
2. Run locally:
   ```bash
   uvicorn main:app --reload --port 8000
   ```
3. Open http://localhost:8000/docs for interactive OpenAPI docs.

## Testing

- Unit tests (in-process):
  ```bash
  pytest
  ```
- Live integration test against a running agent (requires server running and env var):
  ```bash
  uvicorn main:app --reload --port 8000
  set A2A_BASE_URL=http://localhost:8000
  pytest -m integration
  ```

The unit suite exercises metadata, health, validation (missing user message, empty content), and reply shaping (instructions + context hints). The integration test hits a running agent over HTTP to verify end-to-end behavior.

## Call the agent from a script

With the server running (e.g., `uvicorn main:app --reload --port 8000`):

```bash
set A2A_BASE_URL=http://localhost:8000
python test_agent.py
```

The script posts a small conversation to `/a2a/messages` and prints the JSON response.

## VS Code debugging

- Start the "FastAPI: uvicorn (main:app)" configuration in the Run and Debug view. It launches `uvicorn main:app --reload --port 8000` with `A2A_BASE_URL` set for local calls.

## Endpoints

- `GET /a2a/metadata` — static metadata describing this agent service.
- `POST /a2a/messages` — submit the conversation state; returns the assistant reply.
- `POST /a2a/messages/stream` — same as above, but streams the reply in two chunks (demo streaming).
- `GET /healthz` — simple liveness probe.

## Request / Response Examples

### Metadata
```bash
curl http://localhost:8000/a2a/metadata
```

### Send messages
```bash
curl -X POST http://localhost:8000/a2a/messages \
  -H "Content-Type: application/json" \
  -d '{
    "session_id": "demo-session-1",
    "messages": [
      {"role": "system", "content": [{"type": "text", "text": "You are a helpful agent."}]},
      {"role": "user", "content": [{"type": "text", "text": "Hello there!"}]}
    ]
  }'
```

### Stream messages
```bash
curl -N -X POST http://localhost:8000/a2a/messages/stream \
  -H "Content-Type: application/json" \
  -d '{
    "session_id": "demo-session-1",
    "messages": [
      {"role": "user", "content": [{"type": "text", "text": "stream please"}]}
    ]
  }'
```
This will return the assistant reply split across two streamed chunks.

Sample response:
```json
{
  "messages": [
    {
      "role": "assistant",
      "content": [
        {"type": "text", "text": "I am a sample A2A agent. You said: 'Hello there!'. Instructions: none. Context: no context provided."}
      ]
    }
  ],
  "metadata": {
    "received_at": "2026-01-29T00:00:00.000000Z",
    "request_ip": "127.0.0.1",
    "protocol": "a2a-agent-protocol-v1"
  }
}
```

## How the sample works

- Request validation and schema definitions are in `main.py` using Pydantic models.
- `POST /a2a/messages` looks for the latest `user` message and echoes it back with minimal context awareness. Replace `generate_reply` with your own logic (LLM call, tool calling, etc.).

## Extending with Microsoft Agent Framework (optional)

If you want richer orchestration or to call Microsoft Foundry models, add the Agent Framework SDK (preview):
```bash
pip install agent-framework-azure-ai --pre
```
Then swap out `generate_reply` for a call into your agent pipeline.

## Notes

- The service currently handles only text content blocks; extend the `Content` model to add other block types (e.g., images, tool calls) as your protocol variant requires.
- Keep the protocol version string (`a2a-agent-protocol-v1`) aligned with your orchestrator.
- For production, configure logging, authentication, and HTTPS termination in front of the service.
