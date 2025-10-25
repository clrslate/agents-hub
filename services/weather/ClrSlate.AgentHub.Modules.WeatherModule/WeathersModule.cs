using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Modularity;

namespace ClrSlate.AgentHub.Modules.WeatherModule;

[DependsOn(
    typeof(AbpAspNetCoreMvcModule)
)]
public class WeathersModule : AbpModule
{
}