using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ArtificialIntelligence;
using ArtificialIntelligence.Agent;
using ArtificialIntelligence.StateMachine;
using OllamaSharp.Models.Chat;
using OpenAI.Chat;
using UnityEngine;
using Debug = UnityEngine.Debug;

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

    private TalkingAction _talkingAction;

    /// <summary>
    /// If the AIAgent is currently in an active conversation, this will append that conversation as context to subsequent ChatGPT prompts.
    /// If the AIAgent is not in a conversation, nothing is returned.
    /// </summary>
    [AppendContext]
    private List<Message> AppendConversationContext()
    {
       if (CurrentConversation.Count == 0)
           return new List<Message>();

       return new List<Message>
       {
           new()
           {
               Role = ChatRole.System,
               Content = string.Join("\n",
                   CurrentConversation.Select(x => $"{x.Speaker.PlayerInfo.CharacterInformation.Name}: {x.Text}"))
           }
       };
    }
    
    /// <summary>
    /// Speaks to another agent. Adds a single spoken statement to each agents' CurrentConversation.
    /// </summary>
    public async Task<Statement> SpeakTo(IPlayer agent)
    {
        // Speak to the agent and append the statement to the respective conversations
        var prompt = string.Format(Prompt.AgentTalk, agent.PlayerInfo.CharacterInformation.Name);
        var endSignalTool = ChatTool.CreateFunctionTool("end_conversation");
        var completion = await Ollama(prompt, new ChatCompletionOptions()
        {
            ToolChoice = ChatToolChoice.CreateAutoChoice(),
            Tools = { endSignalTool }
        });

        // The conversation continues, append the new goodies to our conversation history
        var words = completion;
        var newStatement = new Statement
        {
            Speaker = this,
            Text = words
        };
        
        CurrentConversation.Add(newStatement);
        
        Debug.Log($"{PlayerInfo.CharacterInformation.Name}: {words}");
        return newStatement;
    }
    
    public void OnTalkedAt(IPlayer other, string message)
    {
        // Add the message to our conversation
        var statement = new Statement
        {
            Speaker = other,
            Text = message
        };
        CurrentConversation.Add(statement);

        // Respond to the person who talked to us
        Task.Run(async () =>
        {
            await Awaitable.MainThreadAsync();
            var myResponse = await SpeakTo(other);

            // Conversation is over, don't invoke their OnTalkedAt
            if (myResponse.Text == EndConvoKeyword)
            {
                _talkingAction.EndConversation();
            }
            else
            {
                other.OnTalkedAt(this, myResponse.Text);
            }
        });
    }

    public void StartTalking(TalkingAction action)
    {
        _talkingAction = action;
        // To begin a conversation, grab a message from ChatGPT and send it to the other agent
        Task.Run(async () =>
        {
            var text = await SpeakTo(action.Other);
            action.Other.OnTalkedAt(this, text.Text);
        });
    }

    public void StopTalking()
    {
        CurrentConversation.Clear();
        _talkingAction = null;
    }
}