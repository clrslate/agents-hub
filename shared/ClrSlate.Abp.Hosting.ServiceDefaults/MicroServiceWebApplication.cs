using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ClrSlate.AgentHub.ServiceDefaults;
using Serilog;
using Serilog.Events;
using Volo.Abp;
using Volo.Abp.Modularity;


namespace ClrSlate.Abp.Hosting.ServiceDefaults;

public static class MicroServiceWebApplication
{
    public static async Task<int> RunMicroserviceAsync(
        string[] args,
        Func<WebApplicationBuilder, Task>? buildConfiguration = null,
        Func<WebApplication, Task>? appConfiguration = null)
    {

        Log.Logger = new LoggerConfiguration()
            .WriteTo.Async(c => c.Console())
            .CreateBootstrapLogger();
        try {
            Log.Information($"Starting Application");

            var builder = WebApplication.CreateBuilder(args);

            builder.AddServiceDefaults();

            builder.Host.
                UseAutofac()
                .UseSerilog((context, services, loggerConfiguration) => {
                    var applicationName = services.GetRequiredService<IApplicationInfoAccessor>().ApplicationName;

                    loggerConfiguration
#if DEBUG
                        .MinimumLevel.Debug()
#else
                .MinimumLevel.Information()
#endif
                        .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                        .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
                        .Enrich.FromLogContext()
                        .Enrich.WithProperty("Application", applicationName)
                        .WriteTo.Async(c => c.Console())
                        .WriteTo.Async(c => c.OpenTelemetry());
                });

            if (buildConfiguration != null) await buildConfiguration(builder);

            var app = builder.Build();

            if (appConfiguration != null) await appConfiguration(app);

            await app.RunAsync();
            Log.Information($"Stopped Application");

            return 0;
        }
        catch (HostAbortedException) {
            /* Ignoring this exception because: https://github.com/dotnet/efcore/issues/29809#issuecomment-1345132260 */
            return 2;
        }
        catch (Exception ex) {

            Console.WriteLine($"Application terminated unexpectedly!");
            Console.WriteLine(ex.ToString());
            Console.WriteLine(ex.StackTrace ?? "");

            Log.Fatal(ex, $"Application terminated unexpectedly!");
            Log.Fatal(ex.Message);
            Log.Fatal(ex.StackTrace ?? "");
            return 1;
        }
        finally {
            await Log.CloseAndFlushAsync();
        }
    }

    public static async Task<int> RunMicroserviceAsync<TStartupModule>(
        string[] args,
        Func<WebApplicationBuilder, Task>? buildConfiguration = null,
        Func<WebApplication, Task>? appConfiguration = null)
            where TStartupModule : IAbpModule
    {
        return await RunMicroserviceAsync(
            args,
            async builder => {
                await builder.AddApplicationAsync<TStartupModule>();
                if (buildConfiguration != null) await buildConfiguration(builder);
            },
            async app => {
                app.UseRouting();
                app.MapDefaultEndpoints();
                await app.InitializeApplicationAsync();
                if (appConfiguration != null) await appConfiguration(app);
            });
    }
}
