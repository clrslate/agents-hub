using Azure.AI.OpenAI;
using ClrSlate.Modules.AgentsAppModule.Options;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;
using OpenAI;
using System.Collections.Concurrent;

namespace ClrSlate.Modules.AgentsAppModule.Abstraction;

public interface IChatClientCatalog
{ 
    Task<IChatClient> GetAsync(string providerName, string key);
}

internal class ChatClientCatalog(IOptions<AiConfigOptions> aiConfigOptions) : IChatClientCatalog
{
    private readonly ConcurrentDictionary<string, IChatClient> _cache = new();
    public async Task<IChatClient> GetAsync(string providerName, string key) 
        => _cache.GetOrAdd($"{providerName}:{key}", CreateChatClient(providerName, key));

    private IChatClient CreateChatClient(string providerName, string key)
    {
        if (providerName == "OpenAI")
        {
            var config = aiConfigOptions.Value.OpenAI[key];
            var client = new OpenAIClient(config.ApiKey);
            var chatClient = client.GetChatClient(config.ModelId).AsIChatClient();
            return chatClient;
        }
        else if (providerName == "AzureOpenAI")
        {
            var config = aiConfigOptions.Value.AzureOpenAI[key];
            var client = new AzureOpenAIClient(
                new Uri(config.Endpoint),
                new System.ClientModel.ApiKeyCredential(config.ApiKey));
            var chatClient = client.GetChatClient(config.DeploymentName).AsIChatClient();
            return chatClient;
        }
        throw new NotImplementedException();
    }
}
