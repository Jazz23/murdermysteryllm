using System.Diagnostics;
using System.Text.Json;
using OpenAI.Chat;

namespace MurderMysteryLLM;

public class AIAgent(ChatClient ChatClient, string name)
{
    public string Name { get; } = name;
    
    /// <summary>
    /// If the agent is actively talking with someone, this stores (in chronological order) the statements made by the agent and the other party.
    /// If the agent is not actively talking with someone, this is null.
    /// </summary>
    public List<Statement>? CurrentConversation { get; private set; }

    public async Task<Statement> SpeakTo(AIAgent agent)
    {
        var testStatement = new Statement(agent.Name, "Hello there, how are you?");
        
        // If first time speaking, initialize conversation
        CurrentConversation ??= [testStatement];
        agent.CurrentConversation ??= [testStatement];
        
        // Speak to the agent and append the statement to the respective conversations
        var newStatement = await SaySomething();
        CurrentConversation.Add(newStatement);
        agent.CurrentConversation.Add(newStatement);
        
        return newStatement;
    }

    private async Task<Statement> SaySomething()
    {
        Debug.Assert(CurrentConversation != null);
        Debug.Assert(CurrentConversation.LastOrDefault()?.Speaker != Name, "It's not our turn to speak.");
        
        var chatMessages = CurrentConversation.Select(ChatMessage (x) =>
        {
            // We are the assistant, the other agent is the user
            if (x.Speaker == Name)
                return new AssistantChatMessage(x.Text);
            return new UserChatMessage(x.Text);
        });
        
        var words = (await ChatClient.CompleteChatAsync(chatMessages)).Value.Content.First().Text;
        return new Statement(Name, words);
    }
    
    public async Task<string> Test() => (await ChatClient.CompleteChatAsync("What is your name?")).Value.Content.First().Text;
}

public record Statement(string Speaker, string Text)
{
    public override string ToString() => $"{Speaker}: {Text}";
}

public enum AgentActions
{
    SEARCH,
    TALK,
    DOOR,
    VOTE
}