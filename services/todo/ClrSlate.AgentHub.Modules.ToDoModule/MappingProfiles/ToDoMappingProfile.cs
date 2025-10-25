using AutoMapper;
using ClrSlate.AgentHub.Modules.ToDoModule.Models;
using Volo.Abp.AutoMapper;

namespace ClrSlate.AgentHub.Modules.ToDoModule.MappingProfiles;

internal class ToDoMappingProfile : Profile
{
    public ToDoMappingProfile()
    {
        CreateMap<TodoItem, ToDoEntity>().IgnoreAuditedObjectProperties()
            .ReverseMap();
    }
}
