# Data Model: AgentDefinition

Domain Entity: `AgentDefinition` (Mongo-backed, ABP audited, soft deletable)
Primary Identifier: `Name` (slug, immutable, lowercase)
Concurrency Field: `Version` (long, increment per mutation)

## Fields
| Field | Type | Required | Immutable | Notes |
|-------|------|----------|----------|-------|
| Name | string | Yes | Yes | Slug pattern ^[a-z0-9]+(?:-[a-z0-9]+)*$, ≤64 chars; unique index |
| DisplayName | string | No | No | Human-readable label; defaults to Name if absent |
| Description | string | No | No | Free-form description |
| Instructions | string | No | No | System prompt / behavior; 0–16000 chars |
| Model | string | Yes | No | Selected model key (provider:model or simple) |
| Version | long | Yes | N/A | Starts at 1, ++ on update/delete |
| IsDeleted | bool | Framework | N/A | ABP soft delete flag |
| CreationTime | DateTime | Framework | Yes | Audit field |
| LastModificationTime | DateTime? | Framework | N/A | For ordering (fallback if custom UpdatedTime not added) |
| CreatorId | Guid? | Framework | N/A | Actor (unused until Auth) |
| LastModifierId | Guid? | Framework | N/A | Actor (unused) |
| DeleterId | Guid? | Framework | N/A | Actor (unused) |
| DeletionTime | DateTime? | Framework | N/A | Soft delete timestamp |

## Invariants
- Name must remain immutable post-creation.
- Version strictly increases by exactly 1 per successful update or soft delete.
- Instructions length ≤ 16000 (after trimming). Empty allowed.
- Model must be non-empty on create and update (if provided).

## Derived / Client Semantics
- `IsDeleted=true` treated as archived; excluded from list by default.
- `LastModificationTime` used for list ordering (desc).

## Indexes (Mongo)
| Index | Keys | Options | Purpose |
|-------|------|---------|---------|
| IX_Agent_Name | { Name: 1 } | unique | Fast lookup & constraint |
| IX_Agent_LastMod | { LastModificationTime: -1 } | sparse? no | Efficient latest-first paging |

(Additional indexes deferred until search or advanced filtering is added.)

## State Transitions
```
Active (IsDeleted=false) --DELETE--> Deleted (IsDeleted=true)
Deleted --(no restore in v1)--> (terminal for v1)
```
All other fields modifiable while Active except Name.

## Concurrency Flow
1. Client fetches details → gets `Version` (and ETag header optionally).
2. Client issues PUT/DELETE with Version.
3. Repository filter matches `{ Name, Version }`.
4. On success update increments Version.
5. On mismatch → no doc modified → 409 with current Version.

## Validation Summary
| Rule | Enforcement |
|------|-------------|
| Slug format | FluentValidation regex |
| Unique name | Repository existence check on create |
| Prompt length | FluentValidation max length 16000 |
| Name immutable | Update validator rejects presence / difference |
| Version required on update/delete | Validator / controller guard |

## DTO Mapping
| DTO Field | Source |
|-----------|--------|
| AgentSummary.Name | AgentDefinition.Name |
| AgentSummary.Model | AgentDefinition.Model |
| AgentSummary.DisplayName | DisplayName ?? Name |
| AgentSummary.UpdatedAtUtc | LastModificationTime | 
| AgentDetails.Instructions | Instructions |
| AgentDetails.Version | Version |
| AgentDetails.IsDeleted (optional) | IsDeleted |

## Open Questions (Not Blocking)
- Should we materialize a separate `UpdatedAtUtc` field to avoid null `LastModificationTime` pre-first update? (Option: if null use CreationTime.)
- Do we later allow partial patch semantics? (Currently full update.)

## Migration Notes
- Add `Version` with default 1 for existing documents (if any) via update script.
- Ensure unique index on Name before enabling write endpoints.

## Future Extensions
- Add `AlternateModels: string[]` (requires raising update semantics; keep Version increment rule).
- Add `Tags: string[]` (introduces array index considerations).
- Add `OwnerId` after Auth spec (GUID) + index.
