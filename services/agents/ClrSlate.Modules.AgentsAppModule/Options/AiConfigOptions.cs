using ClrSlate.Modules.AgentsAppModule.Models;
using System.ComponentModel.DataAnnotations;

namespace ClrSlate.Modules.AgentsAppModule.Options;

internal record AiConfigOptions
{ 
    public const string ConfigSectionName = "Agents";
    public Dictionary<string, AgentDefinition> Definitions { get; set; } = [];
    public Dictionary<string, OpenAIConfig> OpenAI { get; set; } = [];
    public Dictionary<string, AzureOpenAIConfig> AzureOpenAI { get; set; } = [];
}

/// <summary>
/// OpenAI service settings.
/// </summary>
internal sealed class OpenAIConfig
{
    public string? DisplayName { get; set; }

    [Required]
    public required string ModelId { get; set; } = string.Empty;

    [Required]
    public required string ApiKey { get; set; } = string.Empty;
}

/// <summary>
/// Azure OpenAI service settings.
/// </summary>
internal sealed class AzureOpenAIConfig
{

    public string? DisplayName { get; set; }

    [Required]
    public required string DeploymentName { get; set; } = string.Empty;

    [Required]
    public required string Endpoint { get; set; } = string.Empty;

    [Required]
    public required string ApiKey { get; set; } = string.Empty;
}
