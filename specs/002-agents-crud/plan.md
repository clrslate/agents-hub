# Implementation Plan: Agents CRUD (Spec 002)

## Scope Recap
Deliver CRUD (list, create, get, update, soft delete) for `AgentDefinition` with slug primary key, single model selection, optimistic concurrency (Version), soft delete via DELETE.

## Architecture Touch Points
- Module: `ClrSlate.Modules.AgentsAppModule`
- Persistence: Mongo (existing context) + new Version field + indexes
- API Surface: REST under `/api/agents`
- Mapping: AutoMapper profile (entity ↔ DTOs)
- Validation: FluentValidation
- Concurrency: Filter by (Name & Version), increment Version on success

## Phases
### Phase 1: Data & Contracts
1. Add `Version` field to entity + update mapping object (default = 1)
2. Create unique index (Name) + (optional) LastModificationTime index
3. DTOs: `CreateAgentRequest`, `UpdateAgentRequest`, `AgentSummaryDto`, `AgentDetailsDto`
4. Validators for Create & Update
5. AutoMapper Profile
6. OpenAPI doc integration (optional auto-gen vs static file committed)

Exit Criteria:
- Build passes
- Index creation verified at startup (migration log)
- Validation unit tests green

### Phase 2: Repository & Service Layer
Repository Strategy UPDATE (2025-10-26): We will NOT maintain a bespoke `IAgentRepository`; instead we inject `IRepository<AgentDefinition,string>` directly (see `specs/repository-guidelines.md`).

Required service capabilities:
1. Create (uniqueness via unique index; duplicate => 409 at API layer)
2. Get (details, excluding deleted unless explicitly requested by future include flag)
3. List (paged, exclude deleted by default, optional includeDeleted param at controller phase)
4. Update with optimistic concurrency (Version check in service; optional atomic helper if contention justifies)
5. Soft delete with concurrency (increment Version)

Implementation Notes:
- Concurrency: Fetch + compare version; if mismatch throw `ConcurrencyConflictException` (contains current version).
- Atomic path (future): Could be added via extension method on IMongoCollection if required; not mandatory initially.
- Logging: Info on create/update/delete; warn on conflict.

Exit Criteria:
- Unit tests cover success + concurrency conflict + soft delete using generic repository

### Phase 3: API Endpoints
1. Controller `AgentsController`
2. Endpoints implement OpenAPI contract
3. ETag / If-Match handling (Version string) mapping
4. Error responses standardized
5. Logging (info on create/update/delete, warning on conflicts)

Exit Criteria:
- Integration tests (happy path + conflict) pass
- OpenAPI doc served

### Phase 4: Hardening & QA
1. Add pagination bounds tests
2. Load test script (optional placeholder) to assert 10k listing perf assumption
3. Documentation: quickstart & README snippet
4. Metrics hook placeholders (commented or interface) for future instrumentation

Exit Criteria:
- All tests green
- Docs updated

## Non-Goals (Enforced)
- Authentication / Authorization
- Search / filtering beyond paging & includeDeleted flag
- Rename, restore, multi-model

## Risks & Mitigations
| Risk | Mitigation |
|------|------------|
| Missing index leads to duplicate names | Startup index ensure + unique constraint test |
| Race on concurrent create same name | Rely on unique index exception path |
| Lost update on soft delete vs update | Version concurrency safeguards |
| Oversized instructions degrade perf | Validation cap + test |

## Test Matrix (Minimum)
| Case | Description |
|------|-------------|
| Create_OK | Valid create sets Version=1 |
| Create_Duplicate | Duplicate name returns 409 |
| Get_NotFound | Unknown slug 404 |
| Update_VersionOk | Update increments Version |
| Update_Conflict | Stale version returns 409 |
| Delete_Conflict | Stale version on delete 409 |
| Delete_Then_Get | Deleted agent 404 on get (non-includeDeleted) |
| List_ExcludeDeleted | Soft deleted not returned |
| List_IncludeDeleted | Soft deleted included |
| Validation_PromptTooLong | >16000 rejected |

## Metrics (Future Hooks)
- ConcurrencyConflictCount
- CreateLatencyMs
- UpdateLatencyMs

## Completion Definition
Feature complete when all exit criteria across phases satisfied & spec success criteria SC-001..SC-008 demonstrably met.
