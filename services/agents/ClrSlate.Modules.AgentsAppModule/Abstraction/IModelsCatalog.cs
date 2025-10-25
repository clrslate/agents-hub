using Microsoft.Extensions.Options;
using ClrSlate.Modules.AgentsAppModule.Models;
using System.Runtime.CompilerServices;
using ClrSlate.Modules.AgentsAppModule.Options;

namespace ClrSlate.Modules.AgentsAppModule.Abstraction;

public interface IModelsCatalog
{
    Task<IEnumerable<ModelConfig>> GetAllAsync(CancellationToken cancellationToken = default);
}

public interface IModelsRegistry
{
    Task<IEnumerable<ModelConfig>> GetAllAsync(CancellationToken cancellationToken = default);
}

internal class ConfigModelsRegistry : IModelsRegistry
{
    private readonly List<ModelConfig> _models;
    public ConfigModelsRegistry(IOptions<AiConfigOptions> agentsConfig)
    {
        _models = [];
        foreach (var model in agentsConfig.Value.OpenAI) {
            _models.Add(new ModelConfig { Name = model.Key, DisplayName = model.Value.DisplayName, Provider = "OpenAi" });
        }
        foreach (var model in agentsConfig.Value.AzureOpenAI) {
            _models.Add(new ModelConfig { Name = model.Key, DisplayName = model.Value.DisplayName, Provider = "AzureOpenAi" });
        }
    }
    public async Task<IEnumerable<ModelConfig>> GetAllAsync(CancellationToken cancellationToken = default)
        => _models;
}

internal class ModelsCatalog : IModelsCatalog
{
    private readonly IEnumerable<IModelsRegistry> _registries;
    public ModelsCatalog(IEnumerable<IModelsRegistry> registries)
    {
        _registries = registries;
    }
    public async Task<IEnumerable<ModelConfig>> GetAllAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        // merge all registries into a single dictionary
        var models = new List<ModelConfig>();
        foreach (var registry in _registries) {
            var modelDefinitions = await registry.GetAllAsync(cancellationToken);
            foreach (var modelDefinition in modelDefinitions) {
                models.Add(modelDefinition);
            }
        }
        return models;
    }
}
