namespace ClrSlate.Modules.AgentsAppModule.Models;

public record AgentDefinition
{
    public required string Name { get; set; } = default!;
    public string DisplayName { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string Instructions { get; set; } = default!;
    public ModelReference? Model { get; set; }
    public long Version { get; set; } = 1;
}

public record ModelReference
{
    public required string Name { get; set; }
    public required string Provider { get; set; }
}

public record ModelConfig : ModelReference
{
    private string? _displayName;

    public string DisplayName { get => _displayName ?? Name; set => _displayName = value; }
}

