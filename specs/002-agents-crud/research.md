# Research & Decision Log

Feature: Custom Agent Management (CRUD & Model Selection)
Scope: Minimal CRUD + soft delete + optimistic concurrency; no search; slug name primary key; no auth.
Date: 2025-10-25

## 1. Identifier Strategy
- Decision: Use `Name` slug (lowercase, kebab) as primary identifier across API routes.
- Rationale: Simplifies URLs, avoids exposing internal IDs, aligns with human-friendly referencing.
- Alternatives: (a) GUID IDs + slug alias (extra complexity); (b) Numeric incremental ID (not natural in Mongo). Rejected due to overhead and no added value.

## 2. Concurrency Control
- Decision: Optimistic concurrency with monotonically incremented `Version` (long) stored in document and required for update/delete.
- Rationale: Lightweight for low contention domain; no locks; clear 409 semantics.
- Alternatives: (a) Last-write-wins (risk of silent overwrite); (b) Field-level merge (complex); (c) Pessimistic locks (overkill). Rejected.

## 3. Soft Delete vs Archive Endpoint
- Decision: Use HTTP DELETE to set soft delete flag (`IsDeleted` via ABP auditing) instead of custom archive route.
- Rationale: Aligns with REST semantics; reduces endpoint surface area.
- Alternatives: POST /archive; PATCH with status. Rejected as redundant.

## 4. Entity Reuse
- Decision: Reuse existing `AgentDefinition` entity; add `Version` field.
- Rationale: Avoid duplication; leverage existing auditing base class.
- Alternatives: New Mongo document type ("AgentDocument"). Rejected as unnecessary.

## 5. Search Support
- Decision: Omitted (no search param or fuzzy). Only paging list.
- Rationale: Avoid premature complexity; scale target 10k manageable without search.
- Alternatives: Text index or prefix search reserved for future.

## 6. Prompt Length Constraint
- Decision: Max 16,000 characters for `Instructions`.
- Rationale: Balance flexibility and payload size; easy boundary validation.
- Alternatives: 4k (too small), unlimited (risk performance/abuse).

## 7. Naming Immutability
- Decision: `Name` cannot change after creation.
- Rationale: Stable routing key; eliminates cascade updates.
- Alternatives: Rename operation (requires redirect or alias). Deferred.

## 8. Model Association
- Decision: Single `Model` string field now; future multi-model extension via additional collection/array.
- Rationale: YAGNI; simple for v1.
- Alternatives: Array of model references; versioned model config. Deferred.

## 9. Pagination Strategy
- Decision: Simple page & pageSize with Skip/Take; default sort by most recently updated.
- Rationale: Sufficient for <=10k; minimal code.
- Alternatives: Cursor/seek pagination; not needed at this scale yet.

## 10. Index Strategy
- Decision: Unique index on `Name`; optional index on `LastModificationTime` (or `UpdatedAt` if added) for sort.
- Rationale: Ensures performance for primary flows.
- Future: Add compound indexes only if metrics show need.

## 11. Error Model
- Decision: Structured error with `code`, `message`, `details`.
- Rationale: Extensible and client-friendly.
- Alternatives: Plain string or ABP default only; chosen approach is clearer for clients.

## 12. Logging & Audit Without Auth
- Decision: Log actions with placeholder actor field.
- Rationale: Enables future correlation analysis; no identity yet.

## 13. Restoration (Un-Delete)
- Decision: Out of scope; no restore endpoint.
- Rationale: Simplicity; add later if needed; soft delete keeps data.

## 14. Validation Library
- Decision: FluentValidation.
- Rationale: Already aligned with requirement; expressive rule definitions.

## 15. Mapping
- Decision: AutoMapper profile dedicated to Agent DTOs.
- Rationale: Central mapping configuration; reduces manual mapping code.

## 16. Conflict Response Schema
- Decision: 409 with `concurrency_conflict` + current version detail.
- Rationale: Predictable client retry logic.

## Open Questions (Deferred)
| Topic | Note |
|-------|------|
| Ownership / Auth | Will add with separate spec; may introduce `OwnerId`. |
| Model metadata | Possibly external catalog; not needed now. |
| Search UX | Add only if user feedback demands. |
| Restore endpoint | Evaluate after soft delete usage metrics. |

## Summary
All architectural choices favor minimal complexity with forward compatibility. Future enhancements (auth, search, multi-model) have clear insertion points without refactoring core CRUD pathway.
