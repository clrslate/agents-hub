using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ClrSlate.AgentHub.Modules.ToDoModule.Models;
using Volo.Abp.Domain.Repositories;

namespace ClrSlate.AgentHub.ApiService.Controllers;

[ApiController, Route("todo")]
public class ToDoController(IRepository<ToDoEntity, string> repository, IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<IEnumerable<TodoItem>> GetToDoList()
    {
        var items = await repository.GetListAsync();
        return mapper.Map<List<TodoItem>>(items);
    }

    [HttpPost]
    public async Task<TodoItem> Add(TodoItem toDo)
    {
        var entity = mapper.Map<ToDoEntity>(toDo);
        var insertedItem = await repository.InsertAsync(entity);
        return mapper.Map<TodoItem>(insertedItem);
    }
}