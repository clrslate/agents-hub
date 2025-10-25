using ClrSlate.Modules.AgentsAppModule.Abstraction;
using ClrSlate.Modules.AgentsAppModule.Models;
using Microsoft.AspNetCore.Mvc;

namespace ClrSlate.Modules.AgentsAppModule.Controllers;

[ApiController, Route("api/agents")]
public class AgentsController(IAgentsCatalog agentsCatalog) : ControllerBase
{
    [HttpGet]
    public async Task<List<AgentDefinition>> GetToDoList() 
        => await agentsCatalog.GetAllAsync().ToListAsync();
}
