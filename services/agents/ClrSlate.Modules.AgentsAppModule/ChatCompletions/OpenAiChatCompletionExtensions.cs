using OpenAI.Chat;
using System.ClientModel.Primitives;
using System.Diagnostics;

namespace ClrSlate.Modules.AgentsAppModule.ChatCompletions;

public static class OpenAiChatCompletionExtensions
{
    public static async Task<ChatCompletionOptions?> ToChatCompletionOptionsAsync(this string jsonString) 
        => await GetChatCompletionOptions(jsonString, CancellationToken.None);

    private static async Task<ChatCompletionOptions?> GetChatCompletionOptions(string jsonString, CancellationToken cancellationToken)
    {
        var contentStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(jsonString));
        var requestBinary = await BinaryData.FromStreamAsync(contentStream, cancellationToken).ConfigureAwait(false);

        var chatCompletionOptions = new ChatCompletionOptions();
        var chatCompletionOptionsJsonModel = chatCompletionOptions as IJsonModel<ChatCompletionOptions>;
        Debug.Assert(chatCompletionOptionsJsonModel is not null);

        chatCompletionOptions = chatCompletionOptionsJsonModel.Create(requestBinary, ModelReaderWriterOptions.Json);
        return chatCompletionOptions;
    }
}