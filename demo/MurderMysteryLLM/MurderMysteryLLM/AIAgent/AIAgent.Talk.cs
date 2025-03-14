using System.Diagnostics;
using OpenAI.Chat;

namespace MurderMysteryLLM;

// This partial handles all aspects of the agent talking to other agents and or the player
public partial class AIAgent
{
    /// <summary>
    /// If the agent is actively talking with someone, this stores (in chronological order) the statements made by the agent and the other party.
    /// If the agent is not actively talking with someone, this is null.
    /// </summary>
    public List<Statement>? CurrentConversation { get; private set; }

    /// <summary>
    /// If the AIAgent is currently in an active conversation, this will append that conversation as context to subsequent ChatGPT prompts.
    /// If the AIAgent is not in a conversation, nothing is returned.
    /// </summary>
    [AppendContext]
    private List<ChatMessage> AppendConversationContext()
    {
       if (CurrentConversation == null || CurrentConversation.Count == 0)
           return [];
       
       return [new SystemChatMessage(
           string.Join("\n", CurrentConversation.Select(x => $"{x.Speaker}: {x.Text}"))
       )];
    }
    
    /// <summary>
    /// Speaks to another agent. Adds a single spoken statement to each agents' CurrentConversation.
    /// </summary>
    public async Task<Statement> SpeakTo(AIAgent agent)
    {
        // If first time speaking, initialize conversation
        CurrentConversation ??= [];
        agent.CurrentConversation ??= [];
        
        // Speak to the agent and append the statement to the respective conversations
        var prompt = string.Format(Prompt.AgentTalk, agent.CharacterInformation.Name);
        var words = await ChatGPT(prompt);
        
        // If ChatGPT gave a signal instead of words, append that to the statement instance
        Enum.TryParse(typeof(ConversationSignals), words, true, out var signal);
        var newStatement = new Statement(CharacterInformation.Name, words, (ConversationSignals?)signal);
        
        // If ChatGPT gave us a signal instead of words, handle that signal
        if (newStatement.signal != null)
        {
            switch (newStatement.signal)
            {
                case ConversationSignals.ENDED:
                    StopSpeaking();
                    agent.StopSpeaking();
                    break;
            }
        }
        else // Otherwise, add the statement to the conversation
        {
            CurrentConversation.Add(newStatement);
            agent.CurrentConversation.Add(newStatement);
        }
        
        return newStatement;
    }
    
    /// <summary>
    /// Resets the current conversation to null. TODO: Previous messages should already summarized and stored and added for future context.
    /// </summary>
    public void StopSpeaking() => CurrentConversation = null;
}

public enum ConversationSignals
{
    ENDED
}