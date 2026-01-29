# Python A2A Agent Protocol Service

A Python implementation of the A2A (Agent-to-Agent) protocol with two server options:

| Server | File | Description |
|--------|------|-------------|
| **FastAPI (Custom)** | `main.py` | Lightweight FastAPI server with custom session store |
| **A2A SDK (Official)** | `server.py` | Full A2A SDK with `InMemoryTaskStore`, queue manager, push notifications |

---

## Quick Start

### 1. Install Dependencies
```bash
python -m venv .venv
.venv\Scripts\activate        # Windows
# source .venv/bin/activate   # Linux/Mac
pip install -r requirements.txt
```

### 2. Run the Server

#### Option A: FastAPI Server (main.py)
```bash
# Simple run
uvicorn main:app --reload --port 8000

# With concurrency settings
uvicorn main:app --reload --port 8000 --limit-concurrency 1000 --backlog 2048

# Or via Python
python main.py
```
- OpenAPI docs: http://localhost:8000/docs
- Health check: http://localhost:8000/healthz

#### Option B: A2A SDK Server (server.py)
```bash
python server.py
```
- Agent Card: http://localhost:8000/.well-known/agent.json
- Health check: http://localhost:8000/health

### 3. Test the Agent
```bash
set A2A_BASE_URL=http://localhost:8000
python test_agent.py
```

---

## VS Code Debugging

Use **Run and Debug** (F5) and select a configuration:

| Configuration | Description |
|---------------|-------------|
| **FastAPI: main.py (Custom Session Store)** | Runs main.py with uvicorn |
| **A2A SDK Server (In-Memory)** | Runs server.py with in-memory stores |
| **A2A SDK Server (Redis)** | Runs server.py with Redis backend |
| **Run Tests** | Runs pytest |
| **Test Agent Client** | Runs test_agent.py |

---

## Testing

```bash
# Unit tests
pytest

# Verbose output
pytest -v

# Integration tests (requires running server)
set A2A_BASE_URL=http://localhost:8000
pytest -m integration
```

---

## Configuration (Environment Variables)

### Common
| Variable | Default | Description |
|----------|---------|-------------|
| `USE_REDIS` | `false` | Enable Redis storage |
| `REDIS_HOST` | `localhost` | Redis server host |
| `REDIS_PORT` | `6379` | Redis server port |
| `REDIS_PASSWORD` | - | Redis password (optional) |

### main.py Specific
| Variable | Default | Description |
|----------|---------|-------------|
| `A2A_MAX_CONCURRENCY` | `1000` | Max concurrent requests |
| `A2A_BACKLOG` | `2048` | Connection backlog |
| `A2A_SESSION_TTL_MINUTES` | `60` | Session expiry time |

### server.py Specific
| Variable | Default | Description |
|----------|---------|-------------|
| `A2A_HOST` | `0.0.0.0` | Server bind address |
| `A2A_PORT` | `8000` | Server port |

---

## Endpoints

### main.py (FastAPI)
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/a2a/metadata` | Agent metadata |
| POST | `/a2a/messages` | Send messages (persists to session) |
| POST | `/a2a/messages/stream` | Stream response |
| GET | `/a2a/sessions/{id}` | Get session history |
| DELETE | `/a2a/sessions/{id}` | Delete session |
| POST | `/a2a/sessions/cleanup` | Remove stale sessions |
| GET | `/healthz` | Health check |

### server.py (A2A SDK)
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/.well-known/agent.json` | Agent Card (A2A spec) |
| POST | `/` | A2A protocol handler |
| GET | `/health` | Health check |

---

## Request Examples

### Send Messages (main.py)
```bash
curl -X POST http://localhost:8000/a2a/messages ^
  -H "Content-Type: application/json" ^
  -d "{\"session_id\": \"demo\", \"messages\": [{\"role\": \"user\", \"content\": [{\"type\": \"text\", \"text\": \"Hello!\"}]}]}"
```

### Stream Response
```bash
curl -N -X POST http://localhost:8000/a2a/messages/stream ^
  -H "Content-Type: application/json" ^
  -d "{\"session_id\": \"demo\", \"messages\": [{\"role\": \"user\", \"content\": [{\"type\": \"text\", \"text\": \"stream please\"}]}]}"
```

### Get Agent Card (server.py)
```bash
curl http://localhost:8000/.well-known/agent.json
```

---

## Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                     A2A Agent Service                        │
├─────────────────────────────────────────────────────────────┤
│  main.py (FastAPI)           │  server.py (A2A SDK)         │
│  ├─ Custom session store     │  ├─ InMemoryTaskStore        │
│  ├─ InMemory / Redis         │  ├─ RedisTaskStore           │
│  └─ Streaming support        │  ├─ QueueManager             │
│                              │  └─ PushNotificationConfig   │
├─────────────────────────────────────────────────────────────┤
│                     Storage Layer                            │
│  ┌─────────────┐    ┌─────────────┐                         │
│  │  In-Memory  │    │    Redis    │  (horizontal scaling)   │
│  └─────────────┘    └─────────────┘                         │
└─────────────────────────────────────────────────────────────┘
```

---

## Extending

### Add LLM Integration
Replace `generate_reply()` in `main.py` or `_generate_response()` in `server.py` with:
```python
# Microsoft Foundry / Azure OpenAI
pip install agent-framework-azure-ai --pre
```

### Production Checklist
- [ ] Enable Redis for session persistence
- [ ] Configure HTTPS termination
- [ ] Add authentication middleware
- [ ] Set up monitoring/logging
- [ ] Configure rate limiting
Then swap out `generate_reply` for a call into your agent pipeline.

## Notes

- The service currently handles only text content blocks; extend the `Content` model to add other block types (e.g., images, tool calls) as your protocol variant requires.
- Keep the protocol version string (`a2a-agent-protocol-v1`) aligned with your orchestrator.
- For production, configure logging, authentication, and HTTPS termination in front of the service.
