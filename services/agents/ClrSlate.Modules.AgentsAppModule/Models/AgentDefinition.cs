namespace ClrSlate.Modules.AgentsAppModule.Models;

public record AgentDefinition
{
    public required string Name { get; set; } = default!;
    public string DisplayName { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string Instructions { get; set; } = default!;
    public ModelConfig? Model { get; set; }
    public long Version { get; set; } = 1;
}

public record ModelConfig
{
    private string? _displayName;

    public string DisplayName { get => _displayName ?? Name; set => _displayName = value; }
    public required string Name { get; set; }
    public required string Provider { get; set; }
}

