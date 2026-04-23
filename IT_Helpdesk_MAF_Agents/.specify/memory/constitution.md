# IT Helpdesk MAF Agents — Constitution

## Core Principles

### I. Clean Architecture (NON-NEGOTIABLE)
The codebase follows strict Clean Architecture layering: Domain → Application → Adapters → Infrastructure.
- Domain entities (`src/domain/entities/`) and ports (`src/domain/ports/`) have zero external dependencies.
- Application use cases (`src/application/use_cases/`) depend only on domain interfaces (ports), never on concrete adapters.
- Adapters (`src/adapters/`) implement domain ports. Infrastructure (`src/infrastructure/`) wires everything via dependency injection.
- No circular imports across layers. Violations block PR merges.

### II. Test-First / TDD (NON-NEGOTIABLE)
- Tests must be written **before** implementation code. No exceptions.
- Red → Green → Refactor cycle strictly enforced.
- All use cases, value objects, and adapter logic require unit tests under `tests/`.
- Port contracts must have mock-based tests to verify adapter compliance.
- Minimum coverage gate: 80% per module.

### III. Library-First / Anti-Abstraction
- Every new capability is first expressed as a domain port (interface), then implemented as an adapter.
- No speculative abstractions. No wrapper classes for a single concrete implementation.
- YAGNI: only build what the current spec requires.
- Phase -1 Compliance Gate: Before implementation, verify (a) no new projects needed, (b) no unnecessary abstractions introduced.

### IV. Azure-Native Multi-Agent Architecture
- Agent orchestration uses `azure-ai-projects` SDK (`azure.ai.projects.models`).
- Agent adapters live in `src/adapters/agents/`. Each agent exposes a `FunctionTool` via `ToolSet`.
- System prompts live in `src/adapters/agents/prompts/*.md` — never hardcoded in Python.
- Agent routing is controlled by `IntentClassifier` → `ProcessChatUseCase`. Do not add routing logic elsewhere.

### V. Domain Rules Stay in Domain
- Business rules (e.g., `Ticket.update_status()` validating transitions, `ConfidenceScore.is_low_confidence()` threshold) must live in domain value objects and entities.
- No business logic in routers, adapters, or infrastructure.
- `TicketStatus` transitions and confidence thresholds are domain-level constants — not configurable at runtime without an ADR.

### VI. Security by Default
- All user input is validated at the API boundary via Pydantic schemas (`src/infrastructure/api/models/schemas.py`).
- No raw user content is passed to LLM prompts without sanitization.
- OWASP Top 10 checked before every merge (enforced via `spec-kit-security-review` extension).
- No secrets in source code. All credentials via environment variables (see `.env.example`).
- Access control: `UserRole` value object enforces KB access restrictions in `AnswerKBQueryUseCase`.

### VII. Observability and Structured Logging
- All use cases log intent classification results, routing decisions, and confidence scores.
- Use `logging.getLogger(__name__)` — no `print()` in production code.
- Errors propagate as typed exceptions; unhandled exceptions return `500` with a safe message (no stack trace to client).

### VIII. Dependency Injection at the Infrastructure Layer
- All dependencies (repos, services, LLM, notification) are injected via `src/infrastructure/api/dependencies.py`.
- No `new` / direct instantiation of adapters inside use cases or routers.

### IX. Simplicity Gate
- Before adding any new file, ask: can the existing code be extended instead?
- No utility modules unless used by 3+ callers.
- No abstract base classes unless 2+ concrete implementations exist or are planned within the current spec.

## Technology Stack

| Layer | Technology |
|---|---|
| API | FastAPI + Pydantic v2 |
| Agent Framework | Azure AI Projects SDK (`azure-ai-projects`) |
| LLM | Azure OpenAI (GPT-4o via `azure-ai-inference`) |
| KB Search | Azure AI Search (`azure-search-documents`) |
| Persistence | Azure Cosmos DB (NoSQL, `azure-cosmos`) |
| Testing | pytest + pytest-asyncio + unittest.mock |
| Packaging | uv + pyproject.toml / requirements.txt |
| Containerization | Docker + docker-compose |
| CI | GitHub Actions |

## Development Workflow

1. **Discovery** — HVE Core `@dt-coach` or `@prd-builder` for new feature areas.
2. **Specify** — `/speckit.specify` → `/speckit.clarify` → `/speckit.plan` → `/speckit.tasks`.
3. **Implement** — `/speckit.implement` (TDD order: tests first, source second).
4. **Review** — HVE Core `@task-reviewer`, then `@pr-review` before merge.
5. **Security Gate** — `spec-kit-security-review` extension runs on every PR.
6. **CI Gate** — `spec-kit-ci-guard` blocks merge if spec has unresolved `[NEEDS CLARIFICATION]` markers.

## Governance

- This constitution supersedes all other development practices.
- Amendments require: documented rationale + team approval + migration plan for existing code.
- All specs land in `specs/<feature-branch>/`. Specs are versioned alongside code.
- HVE Core tracking artifacts land in `.copilot-tracking/` (gitignored — ephemeral).
- Drift between spec and implementation is detected by `spec-kit-sync` extension post-deployment.

**Version**: 1.0.0 | **Ratified**: 2026-04-23 | **Last Amended**: 2026-04-23
