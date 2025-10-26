using AutoMapper;
using ClrSlate.Modules.AgentsAppModule.MappingProfiles;
using ClrSlate.Modules.AgentsAppModule.Models;
using EntityAgentDefinition = ClrSlate.Modules.AgentsAppModule.Data.Entities.AgentDefinition;

namespace ClrSlate.Modules.AgentsAppModule.Tests.Mapping;

[TestFixture]
public class AgentMappingProfileTests
{
    private IMapper _mapper = null!;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        var cfg = new MapperConfiguration(c => c.AddProfile<AgentMappingProfile>());
        _mapper = cfg.CreateMapper();
    }

    [Test]
    public void Entity_To_Summary_Maps_DisplayName_Fallback()
    {
        var entity = new EntityAgentDefinition {
            Name = "support-bot",
            DisplayName = string.Empty,
            Description = string.Empty,
            Instructions = string.Empty,
            Model = "model-x",
            Version = 5,
            CreationTime = DateTime.UtcNow.AddMinutes(-10)
        };

        var dto = _mapper.Map<AgentSummaryDto>(entity);
        Assert.That(dto.DisplayName, Is.EqualTo(entity.Name));
        Assert.That(dto.Model, Is.EqualTo("model-x"));
    }

    [Test]
    public void Create_To_Entity_Sets_Defaults()
    {
        var req = new CreateAgentRequest { Name = "alpha", Model = "m" };
        var entity = _mapper.Map<EntityAgentDefinition>(req);
        Assert.That(entity.Name, Is.EqualTo("alpha"));
        Assert.That(entity.DisplayName, Is.EqualTo("alpha"));
        Assert.That(entity.Version, Is.EqualTo(1));
    }
}
