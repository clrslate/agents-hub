using ClrSlate.Modules.AgentsAppModule.Data.Entities;
using MongoDB.Driver;
using Volo.Abp.Data;
using Volo.Abp.MongoDB;

namespace ClrSlate.Modules.AgentsAppModule.Data;

[ConnectionStringName(DatabaseName)]
internal class AgentsDbContext : AbpMongoDbContext
{
    public const string DatabaseName = "agents";
    public IMongoCollection<AgentDefinition> Agents => Collection<AgentDefinition>();
}
