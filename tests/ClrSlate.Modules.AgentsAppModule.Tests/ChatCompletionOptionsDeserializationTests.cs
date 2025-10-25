using ClrSlate.Modules.AgentsAppModule.ChatCompletions;
using OpenAI.Chat;
using System.Reflection;

namespace ClrSlate.Modules.AgentsAppModule.Tests;

public class ChatCompletionOptionsDeserializationTests
{
    [Test]
    public async Task SerializeChatCompletionOptions()
    {
        var jsonString = "{\"stream\": true, \"model\": \"pirate\"}";
        var options = await jsonString.ToChatCompletionOptionsAsync();

        Assert.That(options, Is.Not.Null);

        // Verify internal properties "Stream" and "Model"

        const string streamPropName = "Stream";
        var streamProp = typeof(ChatCompletionOptions).GetProperty(streamPropName, BindingFlags.Instance | BindingFlags.NonPublic)
            ?? throw new MissingMemberException(typeof(ChatCompletionOptions).FullName!, streamPropName);
        var streamGetter = streamProp.GetGetMethod(nonPublic: true) ?? throw new MissingMethodException($"{streamPropName} getter not found.");
        var getStreamNullable = streamGetter.CreateDelegate<Func<ChatCompletionOptions, bool?>>();
        var stream = getStreamNullable(options!);

        Assert.That(stream, Is.EqualTo(true));

        const string modelPropName = "Model";
        var modelProp = typeof(ChatCompletionOptions).GetProperty(modelPropName, BindingFlags.Instance | BindingFlags.NonPublic)
            ?? throw new MissingMemberException(typeof(ChatCompletionOptions).FullName!, modelPropName);
        var modelGetter = modelProp.GetGetMethod(nonPublic: true) ?? throw new MissingMethodException($"{modelPropName} getter not found.");
        var getModel = modelGetter.CreateDelegate<Func<ChatCompletionOptions, string?>>();
        var model = getModel(options!);

        Assert.That(model, Is.EqualTo("pirate"));
    }

    [Test]
    public async Task SerializeChatCompletionOptions_WithMessages()
    {
        var jsonString = "{\"stream\": true, \"model\": \"pirate\", \"messages\": [{\"role\": \"user\", \"content\": \"Hello, how are you\"}]}";
        var options = await jsonString.ToChatCompletionOptionsAsync();

        Assert.That(options, Is.Not.Null);

        const string modelPropName = "Messages";
        var messagesProp = typeof(ChatCompletionOptions).GetProperty(modelPropName, BindingFlags.Instance | BindingFlags.NonPublic)
            ?? throw new MissingMemberException(typeof(ChatCompletionOptions).FullName!, modelPropName);
        var messagesGetter = messagesProp.GetGetMethod(nonPublic: true) ?? throw new MissingMethodException($"{modelPropName} getter not found.");
        var getMessages = messagesGetter.CreateDelegate<Func<ChatCompletionOptions, IList<ChatMessage>?>>();
        var messages = getMessages(options!);
        Assert.That(messages, Is.Not.Null);
        Assert.That(messages!.Count, Is.EqualTo(1));

        var userMessage = messages[0] as UserChatMessage;
        Assert.That(userMessage, Is.Not.Null);

        var chatMessageContent = userMessage.Content.First() as ChatMessageContentPart;
        Assert.That(chatMessageContent, Is.Not.Null);

        Assert.That(chatMessageContent.Text, Is.EqualTo("Hello, how are you"));
    }
}
