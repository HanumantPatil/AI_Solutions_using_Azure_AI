# AI Solutions Using Azure AI
Three small samples that showcase Azure AI Agents with vector search (Contoso PizzaBot), a Python Agent-to-Agent protocol service, and a .NET ML regression model exported to ONNX.

## Repository Layout
- [PizzaApp](PizzaApp) — Python agent demo using Azure AI Agents with a custom pizza ordering toolset and Contoso store content.
- [A2AAgent](A2AAgent) — Python Agent-to-Agent protocol service with FastAPI or the official A2A SDK server.
- [ONNX](ONNX) — C# console app that trains a simple regression model with ML.NET and exports to ONNX.

## Prerequisites
- Azure subscription with access to Azure AI Foundry (for Agents and vector stores).
- Python 3.10+ with `pip`.
- .NET 8 SDK for the ONNX sample.
- `PROJECT_CONNECTION_STRING` environment variable pointing to your Azure AI project endpoint (used by the Python samples).

## PizzaApp (Azure AI Agents + Contoso Pizza)
Guided by the in-repo agent persona in [PizzaApp/instructions.txt](PizzaApp/instructions.txt). Tools include:
- Vector search over Contoso Pizza store docs (preloaded `vs_WlkZVc11hBdT6A3zvuDwk5X5`).
- Function tool `calculate_pizza_for_people` for sizing orders.
- MCP tool for Contoso Pizza microservices (menu, toppings, orders).

### Setup
1) Create and activate a virtual environment.
2) Install deps: `pip install -r PizzaApp/requirements.txt`.
3) Set env vars (e.g., in `.env`):
	 - `PROJECT_CONNECTION_STRING=<your-azure-ai-project-endpoint>`
	 - Optional Azure auth vars for `DefaultAzureCredential`.
4) Confirm docs exist in `PizzaApp/documents` (sample Contoso markdown files included).

### Upload or Rebuild the Vector Store
- Use [PizzaApp/add_data.py](PizzaApp/add_data.py) to upload files from `./documents` and create a vector store. Script prints created store and batch IDs.

### Run the Agent Loop
- Execute [PizzaApp/agent.py](PizzaApp/agent.py) to start a CLI chat loop that sends user input to the Azure AI Agent and streams replies.
- The agent auto-approves MCP calls via `MyRunHandler` and uses automatic function calling. Type `exit` or `quit` to stop.

### Notes
- Reuse a single `AIProjectClient` and vector store to avoid unnecessary resource creation.
- Tool responses (prices UTC pickup times) are converted per store in the prompt rules.
- If you change the vector store ID, update `vector_store_id` in `agent.py`.

## A2AAgent (Agent-to-Agent Protocol Service)
Python A2A protocol service using the official `a2a-sdk` Starlette server in [A2AAgent/main.py](A2AAgent/main.py).

### What it provides
- Agent card plus A2A protocol endpoints (send messages, stream replies, fetch sessions, cleanup) with health checks.
- Swappable storage: in-memory by default; Redis when `USE_REDIS=true`.
- CLI tester in [A2AAgent/test_agent.py](A2AAgent/test_agent.py) for quick roundtrips.

### Run
From `A2AAgent` after installing `requirements.txt` in a virtual env:
- `python main.py`

### Test
- Automated: `pytest`
- Manual loop: `python test_agent.py` (set `A2A_BASE_URL` if not localhost)

### Key endpoints
- `/.well-known/agent-card.json`, `/.well-known/agent.json` (legacy), `/` (protocol handler), `/health`

### Config highlights
- Redis: `USE_REDIS=true` with `REDIS_HOST`, `REDIS_PORT`, `REDIS_PASSWORD`
- Host/port: `A2A_HOST`, `A2A_PORT`

### Quick curl
- Agent card: `curl http://localhost:8000/.well-known/agent-card.json`

## ONNX (ML.NET Regression + Export)
Simple ML.NET console app that trains on toy `HealthData` and exports both ZIP and ONNX artifacts.

### Build and Run
- From `ONNX` folder: `dotnet build` then `dotnet run` (uses `net8.0`).
- Outputs:
	- Prediction printed to console for `Freq = 2.5`.
	- Saved model ZIP: `./Models/HealthModel.zip`.
	- ONNX export: `./Models/HealthModel.onnx`.

### Project Files
- [Program.cs](ONNX/Program.cs) contains data classes, pipeline, training, prediction, and model export logic.
- NuGet dependencies resolved via the generated project assets in `obj/`.

## Responsible AI and Diagnostics
- Enable Azure AI diagnostic logging when responses or latency look off (capture the SDK diagnostic string).
- Handle `429` throttling with retries; reuse clients rather than creating new ones per request.

## Quickstart Commands
- Python env: `pip install -r PizzaApp/requirements.txt`
- Run pizza agent: `python PizzaApp/agent.py`
- A2A FastAPI server: `cd A2AAgent && uvicorn main:app --reload --port 8000`
- A2A SDK server: `cd A2AAgent && python server.py`
- Build/run ONNX: `cd ONNX && dotnet run`
