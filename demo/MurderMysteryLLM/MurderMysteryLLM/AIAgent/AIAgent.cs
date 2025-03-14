using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using OpenAI.Chat;

namespace MurderMysteryLLM;

public partial class AIAgent(ChatClient chatClient, CharacterInformation characterInformation, string currentLocation, StoryContext storyContext)
{
    public CharacterInformation CharacterInformation { get; } = characterInformation;
    public string CurrentLocation { get; } = currentLocation;
    
    /// <summary>
    /// Prepends the setup prompt and current game state/history to the prompt and sends it to OpenAI.
    /// </summary>
    /// <returns>The assistant reply from ChatGPT</returns>
    private async Task<ChatCompletion> ChatGPT(string userPrompt, ChatCompletionOptions chatCompletionOptions)
    {
        var context = await BuildChatGPTContext();
        context.Add(new UserChatMessage(userPrompt));
        return await chatClient.CompleteChatAsync(context, chatCompletionOptions);
    }

    /// <summary>
    /// Appends necessary information to chat log, such as the setup prompt and game state.
    /// </summary>
    /*private*/public async Task<List<ChatMessage>> BuildChatGPTContext()
    {
        var context = new List<ChatMessage>();

        var rawCharacterInfo = JsonSerializer.Serialize(CharacterInformation);
        var rawMurderInnocentInstructions =
            CharacterInformation.Murderer ? Prompt.Murderer : Prompt.Innocent;
        var rawStoryContext = JsonSerializer.Serialize(storyContext);

        var setupPrompt = string.Format(Prompt.AgentSetup,
            rawCharacterInfo,
            rawMurderInnocentInstructions,
            rawStoryContext,
            CurrentLocation);
        
        context.Add(new SystemChatMessage(setupPrompt));
        
        // All other custom context methods found in this class
        context.AddRange(AppendContext.GatherContext(this));
        return context;
    }

    /// <summary>
    /// When attached to a method, that method will be invoked during context generation before any chatGPT prompts are made. The return value
    /// is appended to the context used in any ChatGPT prompt.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    private sealed class AppendContext : Attribute
    {
        /// <summary>
        /// Iterates all methods in the AIAgent type and invokes their context methods and returns a squashed list of
        /// return values.
        /// </summary>
        public static List<ChatMessage> GatherContext(AIAgent agent)
        {
            var contextMethods = agent.GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).Where(x => x.GetCustomAttribute<AppendContext>() != null);
            return contextMethods.SelectMany(x => (List<ChatMessage>) x.Invoke(agent, null)!).ToList();
        }
    }
}

public record Statement(string Speaker, string Text)
{
    public override string ToString() => $"{Speaker}: {Text}";
}

public record CharacterInformation(string Name, string Description, uint Age, string Occupation, string[] Traits, bool Murderer);

public enum AgentActions
{
    SEARCH,
    TALK,
    DOOR,
    VOTE
}