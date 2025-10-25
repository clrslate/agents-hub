using Volo.Abp.Domain.Entities.Auditing;

namespace ClrSlate.Modules.AgentsAppModule.Data.Entities;

public class AgentDefinition : FullAuditedAggregateRoot<string>
{
    public string Name { get; set; } = default!;
    public string DisplayName { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string Instructions { get; set; } = default!;
    public string Model { get; set; } = default!;
}
