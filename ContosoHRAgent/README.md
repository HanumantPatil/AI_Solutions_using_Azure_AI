# ContosoHRAgent

.NET-based Contoso HR agent solution with Microsoft Teams integration.

## Project Layout

* `ContosoHRAgent/`: Main ASP.NET Core app (`net10.0`) with Teams and Semantic Kernel integration.
* `M365Agent/`: Microsoft 365 Agents Toolkit project assets and app package files.
* `ContosoHRAgent.slnx`: Solution file for the main project set.

## Prerequisites

* .NET SDK 10.0
* Azure resources for managed identity and AI integrations (depending on your environment)

## Run

```bash
cd ContosoHRAgent/ContosoHRAgent
dotnet run
```

## Build

```bash
cd ContosoHRAgent/ContosoHRAgent
dotnet build
```

## Notes

* App settings are in `appsettings.json` and environment-specific variants.
* Teams and managed identity settings should be configured before running in connected environments.
