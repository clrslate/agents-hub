## Tasks Checklist

All tasks follow required format: - [ ] TNNN [P?] [USx?] Description with file path.

### Phase 1: Setup
- [ ] T001 Create feature task folder structure confirmation (already present) in `specs/002-agents-crud/`
- [ ] T002 Add `Version` property (long, default=1) to persistence entity in `services/agents/ClrSlate.Modules.AgentsAppModule/Data/Entities/AgentDefinition.cs`
- [ ] T003 Add `Version` property to in-memory model (if needed for mapping) in `services/agents/ClrSlate.Modules.AgentsAppModule/Models/AgentDefinition.cs`
- [ ] T004 Rename Mongo collection property `ToDos` to `Agents` in `services/agents/ClrSlate.Modules.AgentsAppModule/Data/AgentsDbContext.cs`
- [ ] T005 Implement Mongo model configuration + unique index on Name in `services/agents/ClrSlate.Modules.AgentsAppModule/Data/AgentsDbContext.cs`
- [ ] T006 Add optional descending index on `LastModificationTime` in `services/agents/ClrSlate.Modules.AgentsAppModule/Data/AgentsDbContext.cs`
- [ ] T007 Create DTOs (CreateAgentRequest, UpdateAgentRequest, AgentSummaryDto, AgentDetailsDto) in `services/agents/ClrSlate.Modules.AgentsAppModule/Controllers/Dto/`
- [ ] T008 [P] Add FluentValidation Create validator in `services/agents/ClrSlate.Modules.AgentsAppModule/Controllers/Validators/CreateAgentRequestValidator.cs`
- [ ] T009 [P] Add FluentValidation Update validator (checks version, no name change) in `services/agents/ClrSlate.Modules.AgentsAppModule/Controllers/Validators/UpdateAgentRequestValidator.cs`
- [ ] T010 Add AutoMapper profile for Agent mappings in `services/agents/ClrSlate.Modules.AgentsAppModule/Abstraction/AgentMappingProfile.cs`
- [ ] T011 Integrate static OpenAPI file serving in `services/agents/ClrSlate.AgentHub.ApiService/Program.cs`
- [ ] T012 [P] Add error response contract type in `services/agents/ClrSlate.AgentHub.ApiService/Controllers/Errors/ErrorResponse.cs`
- [ ] T013 Unit tests for validators & mapping in `tests/ClrSlate.Modules.AgentsAppModule.Tests/Validators/`

### Phase 2: Foundational
- [ ] T014 Define repository interface IAgentRepository in `services/agents/ClrSlate.Modules.AgentsAppModule/Abstraction/IAgentRepository.cs`
- [ ] T015 Implement Mongo repository AgentRepository in `services/agents/ClrSlate.Modules.AgentsAppModule/Data/AgentRepository.cs`
- [ ] T016 Add method: GetByNameAsync (exclude deleted) in `Data/AgentRepository.cs`
- [ ] T017 Add method: GetDetailsIncludingDeletedAsync in `Data/AgentRepository.cs`
- [ ] T018 Add method: ListAsync (paged, includeDeleted) in `Data/AgentRepository.cs`
- [ ] T019 Add method: UpdateWithVersionAsync (atomic filter on Name & Version) in `Data/AgentRepository.cs`
- [ ] T020 Add method: SoftDeleteWithVersionAsync (sets IsDeleted + increments Version) in `Data/AgentRepository.cs`
- [ ] T021 Domain service interface IAgentService in `Abstraction/IAgentService.cs`
- [ ] T022 Domain service implementation AgentService in `services/agents/ClrSlate.Modules.AgentsAppModule/Agents/AgentService.cs`
- [ ] T023 Concurrency conflict exception & mapping in `Agents/AgentService.cs`
- [ ] T024 Logging (create/update/delete + conflict) in `Agents/AgentService.cs`
- [ ] T025 Unit tests repository success paths in `tests/ClrSlate.Modules.AgentsAppModule.Tests/Repository/`
- [ ] T026 Unit tests repository concurrency conflicts in `tests/ClrSlate.Modules.AgentsAppModule.Tests/Repository/`
- [ ] T027 Unit tests AgentService (create/update/delete flows) in `tests/ClrSlate.Modules.AgentsAppModule.Tests/Services/`

### Phase 3: User Story 1 (List Agents) [US1]
- [ ] T028 [US1] Add list projection (entity→summary) in `AgentMappingProfile.cs`
- [ ] T029 [US1] Implement GET /api/agents list endpoint in `services/agents/ClrSlate.Modules.AgentsAppModule/Controllers/AgentsController.cs`
- [ ] T030 [P] [US1] Add includeDeleted query handling & filtering in `AgentsController.cs`
- [ ] T031 [US1] Add pagination bounds enforcement in `AgentsController.cs`
- [ ] T032 [US1] Integration tests list empty vs populated in `tests/ClrSlate.Modules.AgentsAppModule.Tests/Integration/Agents/ListTests.cs`
- [ ] T033 [US1] Integration test pagination > page size in `tests/ClrSlate.Modules.AgentsAppModule.Tests/Integration/Agents/ListPagingTests.cs`

### Phase 4: User Story 2 (Create Agent) [US2]
- [ ] T034 [US2] Implement POST /api/agents in `Controllers/AgentsController.cs`
- [ ] T035 [P] [US2] Enforce slug + uniqueness (repository + validator) in `Controllers/Validators/CreateAgentRequestValidator.cs`
- [ ] T036 [US2] Return Location header + 201 creation logic in `AgentsController.cs`
- [ ] T037 [US2] Integration tests create success & duplicate name in `tests/.../Integration/Agents/CreateTests.cs`
- [ ] T038 [US2] Integration test missing model validation in `tests/.../Integration/Agents/CreateValidationTests.cs`
- [ ] T039 [US2] Index uniqueness verification test (attempt race) in `tests/.../Integration/Agents/CreateRaceTests.cs`

### Phase 5: User Story 3 (Update Agent) [US3]
- [ ] T040 [US3] Implement PUT /api/agents/{name} in `Controllers/AgentsController.cs`
- [ ] T041 [US3] Map If-Match header to Version int in `AgentsController.cs`
- [ ] T042 [P] [US3] Prevent name mutation (validator + controller guard) in `UpdateAgentRequestValidator.cs`
- [ ] T043 [US3] Concurrency 409 response mapping (include current version) in `AgentsController.cs`
- [ ] T044 [US3] Integration tests update success & version increment in `tests/.../Integration/Agents/UpdateTests.cs`
- [ ] T045 [US3] Integration test concurrency conflict (stale version) in `tests/.../Integration/Agents/UpdateConflictTests.cs`
- [ ] T046 [US3] Prompt length boundary (16000 ok, 16001 fail) in `tests/.../Integration/Agents/PromptLengthTests.cs`

### Phase 6: User Story 4 (Delete / Archive Agent) [US4]
- [ ] T047 [US4] Implement DELETE /api/agents/{name} soft delete in `Controllers/AgentsController.cs`
- [ ] T048 [P] [US4] Apply Version check (If-Match header) in `AgentsController.cs`
- [ ] T049 [US4] Exclude deleted from default list (verify) in `AgentsController.cs`
- [ ] T050 [US4] Integration test delete success & 204 in `tests/.../Integration/Agents/DeleteTests.cs`
- [ ] T051 [US4] Integration test delete concurrency conflict in `tests/.../Integration/Agents/DeleteConflictTests.cs`
- [ ] T052 [US4] Integration test includeDeleted=true returns archived in `tests/.../Integration/Agents/DeleteListTests.cs`

### Final Phase: Polish & Cross-Cutting
- [ ] T053 Add standardized error formatter middleware in `ClrSlate.AgentHub.ApiService/Program.cs`
- [ ] T054 [P] Add metrics placeholders (interfaces only) in `services/agents/ClrSlate.Modules.AgentsAppModule/Abstraction/IMetrics.cs`
- [ ] T055 [P] Document deferral of search FR-010 in `specs/002-agents-crud/research.md`
- [ ] T056 Update quickstart examples if any shape changes in `specs/002-agents-crud/quickstart.md`
- [ ] T058 README feature blurb update in `README.md`
 - [ ] T059 Final checklist verification & closeout doc in `specs/002-agents-crud/tasks.md`
 - [ ] T060 CRITICAL Add OpenTelemetry spans for Create/List/Update/Delete + conflict in `AgentsController.cs` & `AgentService.cs`
 - [ ] T061 CRITICAL Emit structured JSON logs (Create/Update/Delete/Conflict) and add log verification test in `tests/.../Integration/Agents/LoggingTests.cs`
 - [ ] T062 CRITICAL Add metrics counters (agent.created, agent.updated, agent.deleted, agent.conflict) with no-op exporter in test & assertion in `tests/.../Integration/Agents/MetricsTests.cs`
 - [ ] T063 CRITICAL Add ETag (Version) response header on GET/POST/PUT/DELETE (where body returned) in `AgentsController.cs`
 - [ ] T064 CRITICAL Add validation & integration test for immutable name (FR-017) in `tests/.../Integration/Agents/ImmutableNameTests.cs`
 - [ ] T065 CRITICAL Add temporary security exception register file `.specify/exceptions/agents-crud-auth-exception.md` documenting expiry & mitigation

## Dependency Graph (Story Order)
US1 (List) → US2 (Create) → US3 (Update) → US4 (Delete)

Lower-level dependencies:
- T002 before any repository/service tasks
- T014..T020 before controller operations (list/create/update/delete)
- T028 before T029 (list endpoint)
- T034 requires validators (T008, T010)
- Update (T040) requires create/list infrastructure done
- Delete (T047) requires update concurrency path

## Parallel Execution Examples
- T008, T009, T010, T011, T012 can proceed in parallel after entity field added (T002)
- T025–T027 test tasks parallelizable after repository + service implementation (T014–T024)
- T035 can run parallel with T034 once base endpoint scaffold (T029) exists
- T042 and T041 parallel post T040 scaffold
- Polish tasks T053–T056 largely parallel except error middleware (T053) should land before final integration re-run

## Implementation Strategy (MVP First)
1. Deliver Phase 1 + US2 minimal path (create + list) for early feedback.
2. Add US3 concurrency update path.
3. Add US4 soft delete.
4. Harden with tests + polish.

## Format Validation
All tasks use: checkbox, sequential T IDs, [P] only on parallel-safe tasks, [USx] only on story phases. Each includes at least one file path.

## Counts
- Total Tasks: 64 (active)
- US1 Tasks: 6 (T028–T033)
- US2 Tasks: 6 (T034–T039)
- US3 Tasks: 7 (T040–T046)
- US4 Tasks: 6 (T047–T052)
- Setup + Foundational + Polish: 39

## MVP Scope Suggestion
Tasks through T039 (List + Create) form the earliest demonstrable MVP.

## Independent Test Criteria Per Story
- US1: Verify list endpoints with empty + populated + pagination.
- US2: Create success, duplicate rejection, model required.
- US3: Update increments version + concurrency 409.
- US4: Delete hides from default list, appears with includeDeleted, concurrency 409.

## Parallel Opportunities Summary
Highlighted with [P]; these can be batched to reduce cycle time after prerequisites.

## Backlog (Deferred)
- [ ] B001 Model availability validator & test (FR-003) – ensure selected model is active.
- [ ] B002 Deprecated model flag projection & indicator in list/detail (FR-006) + tests.
- [ ] B003 Name substring search/filter (FR-010) – reintroduce when needed.
- [ ] B004 Load/performance scenario for 10k agents (SC-007 measurement harness).
 - [ ] B005 Deterministic model ordering (FR-011) – implement sorted model list provider + test.
 - [ ] B006 Empty-state guidance UX (FR-014) – design message & CTA test.

