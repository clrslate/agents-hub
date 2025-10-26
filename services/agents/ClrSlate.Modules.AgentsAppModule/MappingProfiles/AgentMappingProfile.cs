using AutoMapper;
using ClrSlate.Modules.AgentsAppModule.Models; 
using Volo.Abp.AutoMapper;
using EntityAgentDefinition = ClrSlate.Modules.AgentsAppModule.Data.Entities.AgentEntity;

namespace ClrSlate.Modules.AgentsAppModule.MappingProfiles;

public sealed class AgentMappingProfile : Profile
{
    public AgentMappingProfile()
    {
        // Entity -> Summary
        CreateMap<EntityAgentDefinition, AgentSummaryDto>()
            .ForMember(d => d.DisplayName, o => o.MapFrom(s => string.IsNullOrWhiteSpace(s.DisplayName) ? s.Name : s.DisplayName))
            .ForMember(d => d.UpdatedAtUtc, o => o.MapFrom(s => s.LastModificationTime ?? s.CreationTime));

        // Entity -> Details
        CreateMap<EntityAgentDefinition, AgentDetailsDto>()
            .IncludeBase<EntityAgentDefinition, AgentSummaryDto>()
            .ForMember(d => d.Description, o => o.MapFrom(s => string.IsNullOrWhiteSpace(s.Description) ? null : s.Description))
            .ForMember(d => d.Instructions, o => o.MapFrom(s => string.IsNullOrWhiteSpace(s.Instructions) ? null : s.Instructions));

        // Create -> Entity
        CreateMap<CreateAgentRequest, EntityAgentDefinition>()
            .IgnoreAuditedObjectProperties()
            .ForMember(d => d.DisplayName, o => o.MapFrom(s => s.DisplayName ?? s.Name))
            .ForMember(d => d.Description, o => o.MapFrom(s => s.Description ?? string.Empty))
            .ForMember(d => d.Instructions, o => o.MapFrom(s => s.Instructions ?? string.Empty))
            .ForMember(d => d.Model, o => o.MapFrom(s => s.Model))
            .ForMember(d => d.Version, o => o.MapFrom(_ => 1L));

        // Entity -> Details
        CreateMap<EntityAgentDefinition, AgentDefinition>();

        // Update -> Entity
        var updateMap = CreateMap<UpdateAgentRequest, EntityAgentDefinition>()
            .IgnoreAuditedObjectProperties(); // handled explicitly during update logic

        updateMap.ForAllMembers(o => o.Condition((src, dest, srcMember) => srcMember != null));
    }
}
