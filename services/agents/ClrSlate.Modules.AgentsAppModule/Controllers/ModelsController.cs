using ClrSlate.Modules.AgentsAppModule.Models;
using Microsoft.AspNetCore.Mvc;
using ClrSlate.Modules.AgentsAppModule.Abstraction;

namespace ClrSlate.Modules.AgentsAppModule.Controllers;

[ApiController, Route("api/models")]
public class ModelsController(IModelsCatalog modelsCatalog) : ControllerBase
{
    [HttpGet]
    public async Task<IEnumerable<ModelConfig>> GetAll() 
        => await modelsCatalog.GetAllAsync();
}
