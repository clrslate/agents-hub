namespace ClrSlate.Modules.AgentsAppModule.Models;

public record AgentSummaryDto
{
    public required string Name { get; init; }
    public string? DisplayName { get; init; }
    public required string Model { get; init; }
    public DateTime UpdatedAtUtc { get; init; }
}
