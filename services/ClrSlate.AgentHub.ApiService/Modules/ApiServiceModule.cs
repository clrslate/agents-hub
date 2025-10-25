using ClrSlate.Abp.Hosting.ServiceDefaults;
using ClrSlate.Modules.AgentsAppModule;
using ClrSlate.AgentHub.Modules.ToDoModule;
using ClrSlate.AgentHub.Modules.WeatherModule;
using Volo.Abp.Modularity;

namespace ClrSlate.AgentHub.ApiService.Modules;

[DependsOn(
    typeof(WeathersModule),
    typeof(ToDoModule),
    typeof(AgentsApp)
    )]
public class ApiServiceModule : MicroservicesModule
{
}
