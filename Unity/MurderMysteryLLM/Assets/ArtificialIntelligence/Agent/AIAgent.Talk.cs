using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ArtificialIntelligence;
using ArtificialIntelligence.Agent;
using ArtificialIntelligence.StateMachine;
using OpenAI.Chat;
using UnityEngine;

// This partial handles all aspects of the agent talking to other agents and or the player
// TODO: Add these to the IPlayer interface and make them comptable with the story teller and add events for the Unity team to subscribe to
// https://github.com/Jazz23/murdermysteryllm/issues/16
public partial class AIAgent
{
    public static string EndConvoKeyword = "end_conversation";
    /// <summary>
    /// If the agent is actively talking with someone, this stores (in chronological order) the statements made by the agent and the other party.
    /// If the agent is not actively talking with someone, this is null.
    /// </summary>
    public List<Statement> CurrentConversation { get; private set; } = new();
    
    public void InjectTestConversation(List<Statement> conversation) => CurrentConversation = conversation;

    /// <summary>
    /// If the AIAgent is currently in an active conversation, this will append that conversation as context to subsequent ChatGPT prompts.
    /// If the AIAgent is not in a conversation, nothing is returned.
    /// </summary>
    [AppendContext]
    private List<ChatMessage> AppendConversationContext()
    {
       if (CurrentConversation.Count == 0)
           return new List<ChatMessage>();

       return new List<ChatMessage>
       {
           new SystemChatMessage(
               string.Join("\n", CurrentConversation.Select(x => $"{x.Speaker.PlayerInfo.CharacterInformation.Name}: {x.Text}"))
           )
       };
    }
    
    /// <summary>
    /// Speaks to another agent. Adds a single spoken statement to each agents' CurrentConversation.
    /// </summary>
    public async Task<Statement> SpeakTo(IPlayer agent)
    {
        // Speak to the agent and append the statement to the respective conversations
        var prompt = string.Format(Prompt.AgentTalk, agent.PlayerInfo.CharacterInformation.Name);
        var endSignalTool = ChatTool.CreateFunctionTool(EndConvoKeyword);
        var completion = await ChatGPT(prompt, new ChatCompletionOptions()
        {
            ToolChoice = ChatToolChoice.CreateAutoChoice(),
            Tools = { endSignalTool }
        });
        
        // If ChatGPT called our end_conversation function, stop speaking
        if (completion.FinishReason == ChatFinishReason.ToolCalls)
        {
            StopSpeaking();
            return new Statement
            {
                Speaker = this,
                Text = EndConvoKeyword
            };
        }

        // The conversation continues, append the new goodies to our conversation history
        var words = completion.Content.First().Text;
        var newStatement = new Statement
        {
            Speaker = this,
            Text = words
        };
        
        CurrentConversation.Add(newStatement);
        
        return newStatement;
    }

    /// <summary>
    /// Resets the current conversation to null. TODO: Previous messages should already summarized and stored and added for future context.
    /// </summary>
    public void StopSpeaking() => CurrentConversation.Clear();
    
    public void OnTalkedAt(IPlayer other, string message)
    {
        // Add the message to our conversation
        var statement = new Statement
        {
            Speaker = other,
            Text = message
        };
        CurrentConversation.Add(statement);

        Task.Run(async () =>
        {
            await Awaitable.MainThreadAsync();
            var myResponse = await SpeakTo(other);

            if (myResponse.Text == EndConvoKeyword)
            {
                
            }
            else
            {
                other.OnTalkedAt(this, myResponse.Text);
            }
        });
    }

    public void StartTalking(TalkingAction action)
    {
        
    }
}