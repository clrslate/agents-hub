##
# Feature Specification: Custom Agent Management (CRUD & Model Selection)

**Feature Branch**: `002-agents-crud`  
**Created**: 2025-10-25  
**Status**: Draft  
**Input**: User description: "As a user, I want the ability to list, add, update and delete custom agents. I must be able to select the models from the available set of LLM configured in the system."

## User Scenarios & Testing *(mandatory)*

### User Story 1 - List Existing Agents (Priority: P1)

A user views a paginated (or scrollable) list of all available custom agents with key summary attributes (name, selected model, status/active flag, last updated). User can quickly scan and decide which agent to open or manage.

**Why this priority**: Listing is foundational; without visibility no other management action is discoverable.

**Independent Test**: If only listing is implemented, user can still derive value by seeing current configuration of agents.

**Acceptance Scenarios**:
1. **Given** there are existing agents, **When** the user navigates to the agents management view, **Then** a list of agents with name and model displays.
2. **Given** there are no agents, **When** the user opens the agents view, **Then** an empty-state message with a call-to-action to create an agent is shown.
3. **Given** more agents exist than the default page size, **When** the user reaches the end of visible agents, **Then** the user can access the next set (pagination or lazy load).

---

### User Story 2 - Create New Agent with Model Selection (Priority: P1)

A user creates a new custom agent by providing required metadata (name, purpose/description, optional system prompt) and selecting one model from the system's configured set of available LLM models.

**Why this priority**: Creation delivers core value—enables personalized agent definitions.

**Independent Test**: Creation alone is valuable; user can immediately use the new agent elsewhere.

**Acceptance Scenarios**:
1. **Given** the user is on the create form, **When** they enter a unique agent name and select an available model, **Then** the agent is created and appears in the list.
2. **Given** the user submits without selecting a model, **When** validation runs, **Then** a clear error indicates model selection is required.
3. **Given** the user enters a name already in use (case-insensitive), **When** they submit, **Then** a validation error indicates the name conflict.
4. **Given** system has at least one configured model, **When** the user opens model selector, **Then** only currently enabled models are listed.

---

### User Story 3 - Update Existing Agent (Priority: P2)

A user modifies an existing agent's metadata (description, system prompt) and may change the associated model (if allowed). The agent's slug `name` is immutable after creation.

**Why this priority**: Adjusting an agent as needs evolve prevents proliferation of near-duplicates.

**Independent Test**: Updating alone yields value by refining existing agents without recreating them.

**Acceptance Scenarios**:
1. **Given** an existing agent, **When** the user changes its description and saves, **Then** the updated values persist and reflect in listing.
2. **Given** the user attempts to change the agent name (immutable), **When** they submit, **Then** a validation error indicates the name cannot be changed.
3. **Given** the current model is deprecated/unavailable, **When** the user opens edit, **Then** the UI indicates the model status and requires choosing an available alternative before saving.

---

### User Story 4 - Delete (Soft-Archive) Agent (Priority: P3)

A user removes an agent they no longer need.

**Why this priority**: Prevents clutter and minimizes confusion/maintenance overhead.

**Independent Test**: Deletion alone offers value by reducing noise, even if updates were not available.

**Acceptance Scenarios**:
1. **Given** an agent exists, **When** the user confirms archive (soft delete), **Then** the agent is marked inactive and removed from default listing views.
2. **Given** a soft-archive is initiated, **When** the user cancels the confirmation, **Then** the agent remains active and visible.
3. **Given** an agent is referenced in a workflow, **When** the user soft-archives it, **Then** existing references continue to function while the agent is flagged as inactive and unavailable for new assignments.
4. **Given** an archived agent, **When** a user enables the "Show Archived" filter, **Then** the agent appears with clear inactive status.

---

### Edge Cases

- Creating agent when zero models are configured (creation must be blocked with clear guidance).
- Duplicate name differing only by case (should be treated as duplicate per assumption below).
- Model removed after agent creation (flag agent as needing attention on list).
- Large number of agents (scale up to expected 10k) – verify pagination, query indexes, and response time targets.
- Concurrent update: two users editing same agent (optimistic concurrency 409 path).
- Attempt to archive agent currently referenced in automation (allowed; reference continuity ensured, agent flagged inactive).

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: System MUST provide a list view of agents showing at minimum: name, selected model identifier, last updated timestamp.
- **FR-002**: System MUST allow creation of a new agent with required fields: name (unique), model selection (from active configured models).
- **FR-003**: System MUST validate that selected model exists and is active at time of creation/update.
- **FR-004**: System MUST reject creation/update when agent name conflicts (case-insensitive) with an existing agent.
- **FR-005**: System MUST allow updating agent fields: description, system prompt, selected model (if model remains available). The agent name (slug) is immutable after creation (rename deferred to future scope).
- **FR-006**: System MUST indicate when an agent references a model that has been deprecated or disabled (visual/status flag in listing and detail view).
- **FR-007**: System MUST support soft deletion (archival) of agents; archived agents are excluded from default lists, remain available to existing references, and cannot be selected for new associations. Hard deletion is out of scope for this release.
- **FR-009**: System MUST provide meaningful validation error messages for all user-correctable input issues (name conflict, missing model, missing required fields).
- **FR-010 (Deferred)**: (Deferred for prototype) Future iteration will allow filtering or searching agents by name substring; excluded from current prototype scope.
- **FR-011 (Deferred)**: (Deferred for prototype) System SHOULD present enabled model options in a deterministic, user-friendly order (e.g., alphabetic by display name) – postponed until UI refinement phase.
- **FR-012**: System MUST log all create, update, and delete actions with timestamp and actor (for auditability).
- **FR-013**: System MUST enforce exactly one active model association per agent in this release while reserving a forward-compatible structure to allow future addition of alternate models without data migration.
- **FR-014 (Deferred)**: (Deferred for prototype) System SHOULD provide empty-state guidance when no agents exist (e.g., “Create your first custom agent to ...”) – basic empty list acceptable short-term.
 - **FR-015**: System MUST implement optimistic concurrency: each Agent carries a monotonically changing version token (e.g., ETag/rowversion). Update/Archive operations MUST supply the last known version; if mismatch, API returns 409 Conflict with current representation so client can refresh and retry. No server-side field-level merge in v1.
 - **FR-016**: System MUST enforce a maximum system prompt length of 16,000 characters; attempts to create or update with a longer prompt MUST return a validation error specifying the limit. Trailing whitespace MAY be trimmed prior to length evaluation.
	- **FR-017**: System MUST treat agent name (slug) as immutable; any attempt to change it in update operations MUST return a validation error code `NameImmutable`.

### Key Entities *(include if feature involves data)*

 - **Agent**: Represents a configurable AI agent definition chosen by a user; conceptual attributes: unique name (case-insensitive), description, system prompt (0–16,000 chars, optional long text), selected model (reference), status (active/deprecated/attention/inactive), created timestamp, updated timestamp, version token (ETag/rowversion) for optimistic concurrency, flags (deprecated-model-in-use), archived flag (soft delete indicator).
- **Model (Existing Catalog Entry)**: External configured LLM option; conceptual attributes: identifier, display name, status (active/deprecated/disabled), capabilities metadata (abstracted, not enumerated here).

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: A new user can create and see their first agent in under 60 seconds from opening the management view (observed in usability test with minimal guidance).
- **SC-002**: 95% of valid create/update submissions succeed on first attempt without validation errors (after initial learning curve) during a sample of 50 operations.
- **SC-003**: 90% of users in a pilot can locate and open a specific agent within 10 seconds using list or search.
- **SC-004**: 100% of model-deprecated agents are visually flagged in the listing within one refresh cycle of model status change.
 - **SC-005**: Less than 2% of archive attempts result in user abandonment due to unclear messaging in a usability review sample (post-onboarding).
 - **SC-006**: Audit log records timestamp, action type, and agent identifier for 100% of lifecycle operations in a test sample of 30 actions. (Actor/user identity deferred until authentication feature is introduced.)
 - **SC-007**: Listing endpoint (first page default size 25) median server processing time <300ms (P95 <500ms) at dataset size of 10k agents with standard indexing (name, updated timestamp, status).
 - **SC-008**: 100% of system prompt submissions >16,000 characters are rejected with a specific validation error referencing the configured limit.

### Assumptions

- Agent names are globally unique (not per user/tenant) and case-insensitive.
 - Agent names are immutable once created (explicitly reinforcing FR-017).
- Last-write-wins concurrency model; no explicit conflict resolution UI.
 - Single model per agent enforced in v1; data model anticipates future expansion (e.g., alternate_models collection) but remains unused.
 - Listing default page size assumed 25 (not critical to expose unless performance tuning needed).
 - Soft deletion implemented as archival (inactive flag) with optional future hard purge outside scope.
 - Ownership/roles not in scope; all users treated uniformly until separate authentication feature is added.
 - Expected total agent volume <= 10,000 in primary deployment environment (drives indexing & performance targets).
 - Maximum system prompt length fixed at 16,000 characters (storage sized accordingly; no streaming authoring in v1).

### Out of Scope

- Authentication & Authorization (will be specified separately; no user/role distinctions assumed here)
- Hard deletion (physical removal) of agents
- Multi-model assignment per agent
 - Agent name substring search (FR-010 deferred)
	- Deterministic model ordering (FR-011 deferred)
	- Rich empty-state guidance (FR-014 deferred)

## Temporary Security Exception (Constitution Principle 5)

This feature ships initially without authentication/authorization enforcement on the new `/api/agents` endpoints. This constitutes a TEMPORARY EXCEPTION to Constitution Principle **5. Secure-by-Default & Least Privilege**.

**Justification**: Accelerate internal prototype evaluation before auth module is finalized.
**Mitigations**: Feature is intended for non-production / secured internal environment only; logs will clearly mark unauthenticated access; no PII persisted.
**Scope Limit**: Only the Agents CRUD endpoints.
**Expiry**: Must be removed or replaced with proper auth before first production deployment or by 2025-12-01 (whichever comes first).
**Follow-up Tasks**: Add auth layer & remove exception file; update OpenAPI with security scheme.

If deadline passes without remediation, this specification is non-compliant and MUST NOT be promoted.

## Clarifications

### Session 2025-10-25

- Q: What concurrency control strategy should be used for agent updates? → A: Optimistic concurrency with version/ETag (reject on mismatch, 409 Conflict).
- Q: What is the expected upper bound of total agents? → A: Up to 10k agents.
- Q: What is the maximum system prompt length? → A: 16,000 characters.

### Clarification Notes

Prior clarification points resolved (soft deletion, single-model focus, scale, prompt length, concurrency). Authentication & Authorization entirely excluded from this specification and will be addressed separately; no placeholders retained.
