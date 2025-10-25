using System.Text.Json.Serialization;

namespace ClrSlate.Modules.AgentsAppModule.Models;

public class OpenAiChatCompletionRequest
{
    [JsonPropertyName("model")]
    public string? Model { get; set; }

    [JsonPropertyName("max_tokens")]
    public int? MaxTokens { get; set; }

    [JsonPropertyName("temperature")]
    public float Temperature { get; set; } = 1;
}

public class OpenAIModelList
{
    [JsonPropertyName("object")]
    public string Object { get; set; } = "list";

    [JsonPropertyName("data")]
    public OpenAIModelDto[]? Data { get; set; }

    public OpenAIModelList() { }
    public OpenAIModelList(IEnumerable<OpenAIModelDto> models) => Data = models.ToArray();

    public static implicit operator OpenAIModelList(OpenAIModelDto[] models)
        => new(models);
}

public class OpenAIModelDto
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("created")]
    public long Created { get; set; }

    [JsonPropertyName("object")]
    public string Object { get; set; } = "model";

    [JsonPropertyName("owned_by")]
    public string OwnedBy { get; set; } = "ClrSlate";

    public OpenAIModelDto() { }
    public OpenAIModelDto(string modelId) => Id = modelId;
}