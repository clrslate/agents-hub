namespace ClrSlate.Modules.AgentsAppModule.Models;

public sealed record UpdateAgentRequest
{
    // Name is intentionally excluded (immutable)
    public string? DisplayName { get; init; }
    public string? Description { get; init; }
    public string? Instructions { get; init; }
    public required string Model { get; init; } = string.Empty;
    public required long Version { get; init; }
}
