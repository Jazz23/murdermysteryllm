using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using ArtificialIntelligence;
using OpenAI.Chat;

namespace ArtificialIntelligence.Agent;

public partial class AIAgent : IPlayer
{
    public PlayerInfo PlayerInfo { get; }

    public StateMachine.StateMachine StateMachine { get; set; }

    # region Events

    public event Action<PlayerActions> OnTakeTurn;
    // String is which door was chosen
    public event Action<string> OnAskDoor;
    public event Action<string> OnTakeDoor;
    
    #endregion

    private readonly ChatClient _chatClient;
    
    public AIAgent(ChatClient chatClient, PlayerInfo playerInfo)
    {
        _chatClient = chatClient;
        PlayerInfo = playerInfo;
    }
    
    /// <summary>
    /// Prepends the setup prompt and current game state/history to the prompt and sends it to OpenAI.
    /// </summary>
    /// <returns>The assistant reply from ChatGPT</returns>
    private async Task<ChatCompletion> ChatGPT(string userPrompt, ChatCompletionOptions chatCompletionOptions)
    {
        var context = BuildChatGPTContext();
        context.Add(new UserChatMessage(userPrompt));
        return await _chatClient.CompleteChatAsync(context, chatCompletionOptions);
    }

    /// <summary>
    /// Prepends necessary information to chat log, such as the setup prompt and game state.
    /// </summary>
    private List<ChatMessage> BuildChatGPTContext()
    {
        var context = new List<ChatMessage>();

        var rawCharacterInfo = JsonSerializer.Serialize(PlayerInfo.CharacterInformation);
        var rawMurderInnocentInstructions =
            PlayerInfo.CharacterInformation.Murderer ? Prompt.Murderer : Prompt.Innocent;
        var rawStoryContext = JsonSerializer.Serialize(PlayerInfo.StoryContext);

        var setupPrompt = string.Format(Prompt.AgentSetup,
            rawCharacterInfo,
            rawMurderInnocentInstructions,
            rawStoryContext,
            PlayerInfo.CurrentLocation);
        
        context.Add(new SystemChatMessage(setupPrompt));
        
        // All other custom context methods found in this class
        context.AddRange(AppendContext.GatherContext(this));
        return context;
    }
    
    public async Task<PlayerActions> TakeTurn(string prompt)
    {
        // TODO: Use ChatGPT tooling to choose an action https://platform.openai.com/docs/guides/function-calling?api-mode=chat
        var chosenAction = PlayerActions.DOOR;
        OnTakeTurn?.Invoke(chosenAction);
        return PlayerActions.DOOR;
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
            var contextMethods = agent
                .GetType()
                .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                .Where(x => x.GetCustomAttribute<AppendContext>() != null);
            
            return contextMethods.SelectMany(x => (List<ChatMessage>) x.Invoke(agent, null)!).ToList();
        }
    }
}