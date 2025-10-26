using ClrSlate.Modules.AgentsAppModule.Models;
using Volo.Abp.Domain.Entities.Auditing;

namespace ClrSlate.Modules.AgentsAppModule.Data.Entities;

public class AgentEntity : FullAuditedAggregateRoot<string>
{
    public string Name { get; set; } = default!;
    public string DisplayName { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string Instructions { get; set; } = default!;
    public ModelReference Model { get; set; } = default!;
    /// <summary>
    /// Optimistic concurrency version. Starts at 1 and increments on each successful update or soft delete.
    /// </summary>
    public long Version { get; set; } = 1;
}
