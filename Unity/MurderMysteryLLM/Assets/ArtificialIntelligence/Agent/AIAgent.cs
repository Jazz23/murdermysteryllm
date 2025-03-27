using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using ArtificialIntelligence;
using ArtificialIntelligence.Agent;
using ArtificialIntelligence.StateMachine;
using FishNet.Object;
using OpenAI.Chat;
using UnityEngine;
using UnityEngine.AI;

public partial class AIAgent : NetworkBehaviour, IPlayer
{
    public PlayerInfo PlayerInfo { get; set; }
    public ChatClient ChatClient { get; set; }

    public StateMachine StateMachine { get; set; }

    private readonly ChatClient _chatClient;
    private NavMeshAgent _navAgent;

    public void Start()
    {
        _navAgent = GetComponent<NavMeshAgent>();
        _navAgent.updateRotation = false;
        _navAgent.updateUpAxis = false;
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

    public void TurnStart()
    {
        
        // For now, just pick the nearest door and go to it
        var targetDoor = gameObject.GetCurrentLocation().GetComponentsInChildren<Door>()
            .First();
        
        _navAgent.destination = targetDoor.transform.position;
        
        Task.Run(async () =>
        {
            await Awaitable.MainThreadAsync();
            await Awaitable.NextFrameAsync(); // Make sure we start moving
            while (_navAgent.velocity != Vector3.zero)
                await Awaitable.NextFrameAsync();
            
            StateMachine.QueueAction(new DoorAction { Location = targetDoor.location, Player = this });
        });
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