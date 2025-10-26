namespace ClrSlate.Modules.AgentsAppModule.Models;

public sealed record AgentDetailsDto : AgentSummaryDto
{
    public string? Description { get; init; }
    public string? Instructions { get; init; }
    public long Version { get; init; }
    public bool IsDeleted { get; init; }
}
