namespace ClrSlate.Modules.AgentsAppModule.Models;

public sealed record CreateAgentRequest
{
    public required string Name { get; init; } = string.Empty; // slug, immutable
    public string? DisplayName { get; init; }
    public string? Description { get; init; }
    public string? Instructions { get; init; }
    public required string Model { get; init; } = string.Empty;
}
