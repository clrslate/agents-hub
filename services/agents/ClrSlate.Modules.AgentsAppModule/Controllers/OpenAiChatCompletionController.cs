using ClrSlate.Modules.AgentsAppModule.Abstraction;
using ClrSlate.Modules.AgentsAppModule.ChatCompletions;
using ClrSlate.Modules.AgentsAppModule.Models;
using Microsoft.Agents.AI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace ClrSlate.Modules.AgentsAppModule.Controllers;

[ApiController, Route("api/v1")]
public class OpenAiChatCompletionController(IAgentsCatalog agentsCatalog, IChatClientCatalog chatClientCatalog) : ControllerBase
{
    [HttpGet("models")]
    public async Task<OpenAIModelList> GetAll()
    {
        var agents = await agentsCatalog.GetAllAsync();
        var models = agents.Select(a => new OpenAIModelDto(a.Name)).ToArray();
        return models;
    }

    [HttpPost("chat/completions")]
    public async Task<IResult> ChatCompletion(CancellationToken cancellationToken)
    {
        using var reader = new StreamReader(Request.Body);
        var body = await reader.ReadToEndAsync();
        var request = JsonSerializer.Deserialize<OpenAiChatCompletionRequest>(body);
        var allAgents = await agentsCatalog.GetAllAsync();
        var agentDefinition = allAgents.FirstOrDefault(a => a.Name == request?.Model);
        if (agentDefinition is null) return Results.BadRequest("Invalid request payload.");
        if (agentDefinition.Model is null) return Results.BadRequest("Agent not configured properly. Please contact administrator.");

        var chatCompletionOptions = await body.ToChatCompletionOptionsAsync();
        if (chatCompletionOptions is null) {
            return Results.BadRequest("Invalid request payload.");
        }

        var chatClient = await chatClientCatalog.GetAsync(agentDefinition.Model.Provider, agentDefinition.Model.Name);
        var agent = new ChatClientAgent(chatClient, agentDefinition.Instructions, agentDefinition.DisplayName, agentDefinition.Description);

        var chatCompletionsProcessor = new AIAgentChatCompletionsProcessor(agent);

        return await chatCompletionsProcessor.CreateChatCompletionAsync(chatCompletionOptions, cancellationToken).ConfigureAwait(false);
    }
}
