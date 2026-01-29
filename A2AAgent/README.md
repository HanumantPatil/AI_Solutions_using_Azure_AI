# Python A2A Agent (SDK)

A reference A2A agent implemented with the official `a2a-sdk` using Starlette. It includes a sample executor, optional Redis-backed stores, and serves the agent card plus a health endpoint.

## Quick Start
1. Install dependencies
```bash
python -m venv .venv
.venv\Scripts\activate            # Windows
# source .venv/bin/activate        # Linux/Mac
pip install -r requirements.txt
```
2. Run the A2A SDK server
```bash
python server.py
```
- Agent card: http://localhost:8000/.well-known/agent.json
- Health: http://localhost:8000/health

## Testing
```bash
pytest
```

## Configuration
| Variable | Default | Description |
|----------|---------|-------------|
| `USE_REDIS` | `false` | Enable Redis-backed task storage and push config |
| `REDIS_HOST` | `localhost` | Redis host |
| `REDIS_PORT` | `6379` | Redis port |
| `REDIS_PASSWORD` | - | Redis password (optional) |
| `A2A_HOST` | `0.0.0.0` | Server bind address |
| `A2A_PORT` | `8000` | Server port |

## Request Examples
- Agent card
```bash
curl http://localhost:8000/.well-known/agent.json
```
- Health check
```bash
curl http://localhost:8000/health
```

## Extending
Replace `SampleAgentExecutor` in [server.py](server.py) with your agent logic. If you need Redis for scaling, set `USE_REDIS=true` and provide the Redis settings above.
