using AutoMapper;
using ClrSlate.Modules.AgentsAppModule.Abstraction;
using ClrSlate.Modules.AgentsAppModule.Data.Entities;
using ClrSlate.Modules.AgentsAppModule.Models;
using ClrSlate.Modules.AgentsAppModule.Options;
using Microsoft.Extensions.Options;
using Volo.Abp.Domain.Repositories;

namespace ClrSlate.Modules.AgentsAppModule.Services;

internal class ConfigAgentsRegistry : IAgentsRegistry
{
    private readonly IEnumerable<AgentDefinition> _agents;
    public ConfigAgentsRegistry(IOptions<AiConfigOptions> agents)
    {
        _agents = agents.Value.Definitions.Values;
    }
    public async Task<IEnumerable<AgentDefinition>> GetAllAsync(CancellationToken cancellationToken = default) 
        => _agents;
}

internal class MongoAgentsRegistry : IAgentsRegistry
{
    private readonly IRepository<AgentEntity, string> _agentRepository;
    private readonly IMapper _mapper;

    public MongoAgentsRegistry(IRepository<AgentEntity, string> repository, IMapper mapper)
    {
        _agentRepository = repository;
        _mapper = mapper;
    }
    public async Task<IEnumerable<AgentDefinition>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var agents = await _agentRepository.GetListAsync(cancellationToken: cancellationToken);
        return agents.Select(_mapper.Map<AgentDefinition>);
    }
}
