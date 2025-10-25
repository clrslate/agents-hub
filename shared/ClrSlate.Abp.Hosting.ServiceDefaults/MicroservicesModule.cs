using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;
using Volo.Abp.Swashbuckle;

namespace ClrSlate.Abp.Hosting.ServiceDefaults;

[DependsOn(
    typeof(AbpAspNetCoreMvcModule),
    typeof(AbpAutofacModule),
    typeof(AbpSwashbuckleModule)
)]
public abstract class MicroservicesModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var services = context.Services;
        services.AddAbpSwaggerGen(options => {
            options.HideAbpEndpoints();
        });
        ConfigureConventionalControllers();
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        var app = context.GetApplicationBuilder();
        app.UseRouting();
        app.UseStaticFiles();
        app.UseSwagger();
        app.UseAbpSwaggerUI();
        app.UseConfiguredEndpoints();
    }

    private void ConfigureConventionalControllers()
        => Configure<AbpAspNetCoreMvcOptions>(options => options.ConventionalControllers.Create(this.GetType().Assembly));
}
