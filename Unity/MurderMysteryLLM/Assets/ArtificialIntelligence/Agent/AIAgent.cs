﻿using System;
using System.Reflection;
using System.Text.Json;
using ArtificialIntelligence.Agent;
using ArtificialIntelligence.StateMachine;
using OllamaSharp;
using OllamaSharp.Models.Chat;
using OpenAI.Chat;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public partial class AIAgent : MonoBehaviour, IPlayer
{
    public PlayerInfo PlayerInfo { get; set; }
    public OllamaApiClient ChatClient { get; set; }

    private AgentMover _agentMover;

    [SerializeField]
    private SpriteRenderer _spriteRender;


    [SerializeField]
    public TextMeshProUGUI uiDescriptorText;

    [SerializeField]
    public RawImage speechBubble;


    public void Start()
    {
        _agentMover = GetComponent<AgentMover>();
        
        _spriteRender = GetComponent<SpriteRenderer>();
        _spriteRender.color = new Color(UnityEngine.Random.Range(0, 1), UnityEngine.Random.Range(0, 1), UnityEngine.Random.Range(0, 1));

        uiDescriptorText = GetComponentInChildren<TextMeshProUGUI>();
        speechBubble = GetComponentInChildren<RawImage>();
        speechBubble.enabled = false;

    }

    /// <summary>
    /// Prepends the setup prompt and current game state/history to the prompt and sends it to OpenAI.
    /// </summary>
    /// <returns>The assistant reply from ChatGPT</returns>
    private async Task<string> Ollama(string userPrompt)
    {
        try
        {
            var context = BuildChatGPTContext();
            context.Add(new Message()
            {
                Role = ChatRole.User,
                Content = userPrompt
            });
            
            var response = "";
            await foreach (var stream in ChatClient.ChatAsync(new ChatRequest()
                           {
                               Messages = context
                           }))
                response += stream.Message.Content;
            return response;
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError($"ChatGPT error: {e.Message}");
            throw;
        }
    }

    /// <summary>
    /// Prepends necessary information to chat log, such as the setup prompt and game state.
    /// </summary>
    private List<Message> BuildChatGPTContext()
    {
        var context = new List<Message>();

        var rawCharacterInfo = JsonSerializer.Serialize(PlayerInfo.CharacterInformation);
        var rawMurderInnocentInstructions =
            PlayerInfo.CharacterInformation.Murderer ? Prompt.Murderer : Prompt.Innocent;
        var rawStoryContext = JsonSerializer.Serialize(PlayerInfo.StoryContext);

        var setupPrompt = string.Format(Prompt.AgentSetup,
            rawCharacterInfo,
            rawMurderInnocentInstructions,
            rawStoryContext,
            PlayerInfo.CurrentLocation);

        context.Add(new Message()
        {
            Role = ChatRole.System,
            Content = setupPrompt
        });

        // All other custom context methods found in this class
        context.AddRange(AppendContext.GatherContext(this));
        return context;
    }

    public void TurnStart()
    {
        Task.Run(async () =>
        {
            // Just in case we need to interact with Unity during this turn, use the main thread
            await Awaitable.MainThreadAsync();

            // Wait for any past summarizing tasks to finish before starting our turn
            if (_summarizeCompletionSource != null)
                await _summarizeCompletionSource.Task;

            // var agentToTalkTo = AIInterface.Agents.First(x => x != this);
            // AIInterface.TurnStateMachine.QueueAction(new TalkingAction()
            // {
            //     Other = agentToTalkTo,
            //     Player = this,
            // });
            
            _agentMover.GotoLocation(2);
            AIInterface.TurnStateMachine.SetState(new PickPlayerState());
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
        public static List<Message> GatherContext(AIAgent agent)
        {
            var contextMethods = agent
                .GetType()
                .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                .Where(x => x.GetCustomAttribute<AppendContext>() != null);

            return contextMethods.SelectMany(x => (List<Message>)x.Invoke(agent, null)!).ToList();
        }
    }


}