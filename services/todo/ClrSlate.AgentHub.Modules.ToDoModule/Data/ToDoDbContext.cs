using ClrSlate.AgentHub.Modules.ToDoModule.Models;
using MongoDB.Driver;
using Volo.Abp.Data;
using Volo.Abp.MongoDB;

namespace ClrSlate.AgentHub.Modules.ToDoModule.Data;

[ConnectionStringName(DatabaseName)]
public class ToDoDbContext : AbpMongoDbContext
{
    public const string DatabaseName = "agents";
    public IMongoCollection<ToDoEntity> ToDos => Collection<ToDoEntity>();
}