using ClrSlate.Modules.AgentsAppModule.Abstraction;
using ClrSlate.Modules.AgentsAppModule.Data;
using ClrSlate.Modules.AgentsAppModule.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;
using Volo.Abp.MongoDB;
using ClrSlate.Modules.AgentsAppModule.Services;

namespace ClrSlate.Modules.AgentsAppModule;

[DependsOn(
    typeof(AbpAspNetCoreMvcModule),
    typeof(AbpMongoDbModule),
    typeof(AbpAutoMapperModule)
)]
public class AgentsApp : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var services = context.Services;

        services.AddTransient<IAgentsRegistry, ConfigAgentsRegistry>();
        services.AddTransient<IAgentsRegistry, MongoAgentsRegistry>();
        services.AddTransient<IAgentsCatalog, AgentsCatalog>();

        services.AddTransient<IModelsRegistry, ConfigModelsRegistry>();
        services.AddTransient<IModelsCatalog, ModelsCatalog>();

        services.AddSingleton<IChatClientCatalog, ChatClientCatalog>();
        services.Configure<AiConfigOptions>(context.Configuration.GetSection(AiConfigOptions.ConfigSectionName));
        Configure<AbpAutoMapperOptions>(options => {
            options.AddMaps<AgentsApp>();
        });
        services.AddMongoDbContext<AgentsDbContext>(options => {
            options.AddDefaultRepositories();
        });
        base.ConfigureServices(context);
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        var app = context.GetApplicationBuilder();

        app.UseEndpoints(endpoints => {
        });

        base.OnApplicationInitialization(context);
    }
}
