using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using OpenAI.Chat;

namespace MurderMysteryLLM;

public class AIAgent(ChatClient chatClient, CharacterInformation characterInformation, string currentLocation)
{
    public CharacterInformation CharacterInformation { get; } = characterInformation;
    
    /// <summary>
    /// If the agent is actively talking with someone, this stores (in chronological order) the statements made by the agent and the other party.
    /// If the agent is not actively talking with someone, this is null.
    /// </summary>
    public List<Statement>? CurrentConversation { get; private set; }
    
    /// <summary>
    /// Speaks to another agent. Adds the spoken statement to each agent's CurrentConversation.
    /// </summary>
    public async Task<Statement> SpeakTo(AIAgent agent)
    {
        // Temporary variable since we don't have the setup prompt yet
        var testStatement = new Statement(agent.CharacterInformation.Name, "Hello there, how are you?");
        
        // If first time speaking, initialize conversation
        CurrentConversation ??= [testStatement];
        agent.CurrentConversation ??= [testStatement];
        
        // Speak to the agent and append the statement to the respective conversations
        var newStatement = await SaySomething();
        CurrentConversation.Add(newStatement);
        agent.CurrentConversation.Add(newStatement);
        
        return newStatement;
    }
    
    /// <summary>
    /// Resets the current conversation to null. TODO: Previous messages were already summarized and stored.
    /// </summary>
    public void StopSpeaking() => CurrentConversation = null;

    private async Task<Statement> SaySomething()
    {
        Debug.Assert(CurrentConversation != null);
        Debug.Assert(CurrentConversation.LastOrDefault()?.Speaker != CharacterInformation.Name, "It's not our turn to speak.");
        
        var chatMessages = CurrentConversation.Select(ChatMessage (x) =>
        {
            // We are the assistant, the other agent is the user
            if (x.Speaker == CharacterInformation.Name)
                return new AssistantChatMessage(x.Text);
            return new UserChatMessage(x.Text);
        });
        
        var words = (await chatClient.CompleteChatAsync(chatMessages)).Value.Content.First().Text;
        return new Statement(CharacterInformation.Name, words);
    }

    /// <summary>
    /// Appends necessary information to chat log, such as the setup prompt and game state.
    /// </summary>
    private async Task<List<ChatMessage>> BuildContext()
    {
        var context = new List<ChatMessage>();

        var rawPrompt = await Helpers.ReadFileFromRoot("AgentPrompts/agentSetup");
        // TODO
        return context;
    }
}

public record Statement(string Speaker, string Text)
{
    public override string ToString() => $"{Speaker}: {Text}";
}

public record CharacterInformation(string Name, string Description, uint Age, string Occupation, string[] Traits);

public enum AgentActions
{
    SEARCH,
    TALK,
    DOOR,
    VOTE
}