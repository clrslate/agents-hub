using ClrSlate.Modules.AgentsAppModule.Abstraction;
using ClrSlate.Modules.AgentsAppModule.Models;

namespace ClrSlate.Modules.AgentsAppModule.Services;

internal class AgentsCatalog : IAgentsCatalog
{
    private readonly IEnumerable<IAgentsRegistry> _registries;
    public AgentsCatalog(IEnumerable<IAgentsRegistry> registries) => _registries = registries;

    public async Task<IEnumerable<AgentDefinition>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var agentsTasks = _registries.Select(async r => await r.GetAllAsync(cancellationToken));
        var agentsResults = await Task.WhenAll(agentsTasks);
        return agentsResults.Where(r => r != null).SelectMany(a => a).Where(a => a != null).ToList();
    }
}
