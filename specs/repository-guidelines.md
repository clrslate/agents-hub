# Repository & Persistence Guidelines

Date: 2025-10-26
Applies To: All modules (initially Agents CRUD / `ClrSlate.Modules.AgentsAppModule`)

## Decision
We PREFER using the ABP generic repository interface `IRepository<TEntity, TKey>` (from `Volo.Abp.Domain.Repositories`) instead of creating a bespoke repository interface & implementation per aggregate, unless a clear cross-cutting need emerges (e.g. complex specification reuse across multiple services that cannot be expressed cleanly with LINQ / Mongo filters or extension methods).

## Rationale
1. Reduces boilerplate and proliferation of thin pass-through repository classes.
2. Keeps data access simple and discoverable; developers already know the ABP abstractions.
3. Encourages expressing queries as LINQ or Mongo driver filters close to the application/service logic.
4. Lowers maintenance cost when schema fields evolve.

## When a Custom Repository MAY Be Justified
| Scenario | Evaluate Custom Repo? | Notes |
|----------|----------------------|-------|
| Complex multi-field filtering reused across >3 services | Yes | Consider extension methods first. |
| Aggregated projections requiring server-side pipelines (Map/Reduce, aggregation framework) | Yes | Start with extension methods returning `IFindFluent` or `IQueryable`. |
| Transactional batch operations / Unit-of-Work orchestration beyond basic ABP support | Yes | Keep narrowly scoped. |
| Pure CRUD + simple paging/sorting | No | Use generic repository. |

## Patterns for Agents Module (Mongo)
We retain optimistic concurrency on `AgentDefinition.Version`. Instead of a custom `IAgentRepository`, service-layer code SHOULD:

1. Fetch the current document by Name via `IRepository<AgentDefinition,string>.FirstOrDefaultAsync(x => x.Name == name && !x.IsDeleted)`.
2. Perform concurrency check in service (`if (entity.Version != expectedVersion) throw ConcurrencyConflict`).
3. Apply mutations and increment Version in-memory.
4. Persist with `UpdateAsync(entity)` (ABP sets the entire document) OR use `IMongoCollection` for atomic filter if strict atomicity is required.

### Atomic Version Update Helper
For stricter race guarantees (avoid TOCTOU between fetch and replace) create focused extension methods on `IMongoCollection<AgentDefinition>` that perform:
```
var filter = Builders<AgentDefinition>.Filter.Eq(x => x.Name, name) &
             Builders<AgentDefinition>.Filter.Eq(x => x.Version, expectedVersion) &
             Builders<AgentDefinition>.Filter.Eq(x => x.IsDeleted, false);
var update = Builders<AgentDefinition>.Update
    .Set(x => x.DisplayName, newValue)
    .Inc(x => x.Version, 1)
    .CurrentDate(x => x.LastModificationTime);
```
Return success flag + current version when conflict occurs.

We can expose this via an internal static class (see future `AgentRepositoryExtensions.cs`) without introducing a dedicated repository type.

## Soft Delete
Use the ABP audited `IsDeleted` flag. Default list queries MUST exclude deleted unless `includeDeleted=true` parameter explicitly supplied by API layer.

## Paging & Sorting
Standard pattern:
```
query = query.Where(x => !x.IsDeleted);
query = query.OrderByDescending(x => x.LastModificationTime ?? x.CreationTime)
             .Skip(skip).Take(takeBounded);
```
Enforce server-side maximum page size (e.g. 200) centrally via a constant.

## Concurrency Conflict Representation
Throw a module-specific `ConcurrencyConflictException` (already defined in Agents service) including `Name`, `ExpectedVersion`, `CurrentVersion`.

## Migration Strategy (Current Code)
The previously added `IAgentRepository` & `AgentRepository` are candidates for removal/refactor. New code SHOULD inject `IRepository<AgentDefinition,string>` directly. Existing tests will be adapted to use generic repository + extension helpers in a subsequent refactor pass.

## Action Items
- Add `[Obsolete]` attribute to `IAgentRepository` (temporary until deleted).
- Update `plan.md` to reflect decision (Phase 2 no longer requires custom repository class).
- Future: Introduce `AgentRepositoryExtensions` if atomic helpers needed in multiple services.

## Example Service Constructor (Preferred)
```csharp
public class AgentService(
    IRepository<AgentDefinition, string> repo,
    IMapper mapper,
    ILogger<AgentService> logger) : IAgentService { /* ... */ }
```

## Summary
Centralizing this guideline prevents divergence across modules (e.g. ToDo module already uses generic repository). Deviations now require justification in PR description referencing this document.
