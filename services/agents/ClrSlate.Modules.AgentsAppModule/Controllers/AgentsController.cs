using AutoMapper;
using ClrSlate.Modules.AgentsAppModule.Abstraction;
using ClrSlate.Modules.AgentsAppModule.Data.Entities;
using ClrSlate.Modules.AgentsAppModule.Models;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Domain.Repositories;

namespace ClrSlate.Modules.AgentsAppModule.Controllers;

[ApiController, Route("api/agents")]
public class AgentsController(IAgentsCatalog agentsCatalog, IRepository<AgentEntity, string> repository, IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<IEnumerable<AgentSummaryDto>> GetToDoList()
    {
        var agents = await agentsCatalog.GetAllAsync();
        return [.. agents.Select(mapper.Map<AgentSummaryDto>)];
    }

    [HttpPost]
    public async Task<AgentSummaryDto> Add(CreateAgentRequest toDo)
    {
        var entity = mapper.Map<AgentEntity>(toDo);
        var insertedItem = await repository.InsertAsync(entity);
        return mapper.Map<AgentSummaryDto>(insertedItem);
    }

    [HttpPut("{id}")]
    public async Task<AgentSummaryDto> Update(string id, UpdateAgentRequest updateRequest)
    {
        var entity = await repository.GetAsync(id);
        mapper.Map(updateRequest, entity);
        var updatedEntity = await repository.UpdateAsync(entity);
        return mapper.Map<AgentSummaryDto>(updatedEntity);
    }

    [HttpDelete("{id}")]
    public async Task Delete(string id) => await repository.DeleteAsync(id);
}
