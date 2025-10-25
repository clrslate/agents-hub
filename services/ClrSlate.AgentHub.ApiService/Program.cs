using ClrSlate.Abp.Hosting.ServiceDefaults;
using ClrSlate.AgentHub.ApiService.Modules;

await MicroServiceWebApplication.RunMicroserviceAsync<ApiServiceModule>(args);