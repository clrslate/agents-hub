using Microsoft.Extensions.Options;
using ClrSlate.Modules.AgentsAppModule.Models;
using System.Runtime.CompilerServices;
using ClrSlate.Modules.AgentsAppModule.Options;

namespace ClrSlate.Modules.AgentsAppModule.Abstraction;

public interface IAgentsCatalog
{
    IAsyncEnumerable<AgentDefinition> GetAllAsync(CancellationToken cancellationToken = default);
}

public interface IAgentsRegistry
{
    IAsyncEnumerable<AgentDefinition> GetAllAsync(CancellationToken cancellationToken = default);
}

internal class ConfigAgentsRegistry : IAgentsRegistry
{
    private readonly IEnumerable<AgentDefinition> _agents;
    public ConfigAgentsRegistry(IOptions<AiConfigOptions> agents)
    {
        _agents = agents.Value.Definitions.Values;
    }
    public IAsyncEnumerable<AgentDefinition> GetAllAsync(CancellationToken cancellationToken = default) 
        => _agents.ToAsyncEnumerable();
}

internal class AgentsCatalog : IAgentsCatalog
{
    private readonly IEnumerable<IAgentsRegistry> _registries;
    public AgentsCatalog(IEnumerable<IAgentsRegistry> registries)
    {
        _registries = registries;
    }
    public async IAsyncEnumerable<AgentDefinition> GetAllAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        foreach (var registry in _registries)
        {
            var agentDefinitions = registry.GetAllAsync(cancellationToken);
            await foreach (var agentDefinition in agentDefinitions)
            {
                yield return agentDefinition;
            }
        }
    }
}
