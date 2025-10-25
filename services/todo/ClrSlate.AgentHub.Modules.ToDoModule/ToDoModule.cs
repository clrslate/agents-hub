using Microsoft.Extensions.DependencyInjection;
using ClrSlate.AgentHub.Modules.ToDoModule.Data;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;
using Volo.Abp.MongoDB;

namespace ClrSlate.AgentHub.Modules.ToDoModule;

[DependsOn(
    typeof(AbpAspNetCoreMvcModule),
    typeof(AbpMongoDbModule),
    typeof(AbpAutoMapperModule)
)]
public class ToDoModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var services = context.Services;

        Configure<AbpAutoMapperOptions>(options => {
            options.AddMaps<ToDoModule>();
        });
        services.AddMongoDbContext<ToDoDbContext>(options =>
        {
            options.AddDefaultRepositories();
        });
        base.ConfigureServices(context);
    }
}