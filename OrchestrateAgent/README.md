# OrchestrateAgent

Multi-project .NET solution following a layered architecture with Web API, identity, persistence, and test projects.

## Structure

* `Agents/Agents.sln`: Main solution
* `Agents/Src/Presentation/Agents.WebApi`: API host project (`net10.0`)
* `Agents/Src/Core`: Domain and application layers
* `Agents/Src/Infrastructure`: Identity, persistence, and shared resources
* `Agents/Tests`: Unit, integration, and functional tests

## Prerequisites

* .NET SDK 10.0
* SQL Server (if `UseInMemoryDatabase` is set to `false`)

## Run Web API

```bash
cd OrchestrateAgent/Agents/Src/Presentation/Agents.WebApi
dotnet run
```

## Build Entire Solution

```bash
cd OrchestrateAgent/Agents
dotnet build Agents.sln
```

## Test

```bash
cd OrchestrateAgent/Agents
dotnet test Agents.sln
```
