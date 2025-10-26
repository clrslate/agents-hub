using ClrSlate.Modules.AgentsAppModule.Models;

namespace ClrSlate.Modules.AgentsAppModule.Abstraction;

public interface IAgentsRegistry
{
    Task<IEnumerable<AgentDefinition>> GetAllAsync(CancellationToken cancellationToken = default);
}
