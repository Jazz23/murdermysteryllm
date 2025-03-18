using System;
using System.Collections;
using System.IO;
using System.Threading.Tasks;
using ArtificialIntelligence;
using ArtificialIntelligence.Agent;
using dotenv.net;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using OpenAI.Chat;
using UnityEngine;

/// <summary>
/// Used to interface Unity with AI library. This starts a Coroutine to drive the Storyteller and AIAgents.
/// </summary>
public class AIInterface : NetworkBehaviour
{
    // Synchronize the server's StoryContext to the rest of the clients
    public static StoryContext StoryContext
    {
        get => _instance?._syncedStoryContext?.Value;
        private set => _instance._syncedStoryContext.Value = value;
    }
    public static ChatClient ChatClient;

    public bool MockStoryContext = true;
    public bool MockPlayerInfo = true;

    private Storyteller _storyteller;
    private readonly SyncVar<StoryContext> _syncedStoryContext = new();
    private static AIInterface _instance;

    public override void OnStartNetwork()
    {
        // This singleton is only attached to one empty (AIInterface) object at the root of the scene
        _instance = this;
    }
    
    public override async void OnStartServer()
    {
        // Only the server should mess around with the AI library
        await InitAILibrary();
    }
    
    // Loads prompts and any testing data
    [Server]
    private async Awaitable InitAILibrary()
    {
        // Place .env in root of unity project
        DotEnv.Load();
        
        ChatClient = new ChatClient("gpt-4o-mini", Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
        
        // Load prompts from local text files
        var promptPathPrefix = Environment.GetEnvironmentVariable("PROMPTS_PATH")!;
        await Prompt.LoadPrompts(promptPathPrefix);

        if (MockStoryContext)
        {
            // If we're mocking story context, load from a file instead of generating it
            StoryContext = await Helpers.CreateStoryContextFromJsonFile(
                Path.Combine(promptPathPrefix, "StorytellerPrompts/storyObject.eg.jsonc")); // Temporary: This will be generated via ChatGPT in the future
        }

        _storyteller = new Storyteller();

        await PlayLoop();
    }

    [Server]
    public static async Awaitable<PlayerInfo> GetPlayerInfo()
    {
        // if (_instance.MockPlayerInfo)
        // {
            var promptPathPrefix = Environment.GetEnvironmentVariable("PROMPTS_PATH")!;
            return await Helpers.GetPlayerInfoFromJsonFile(promptPathPrefix + "AgentPrompts/ExampleData/character.jsonc", "Grand Library",
                StoryContext);
        // }
    }

    /// <summary>
    /// Loops between players and takes their turn via the storyteller.
    /// </summary>
    /// <returns></returns>
    [Server]
    private async Awaitable PlayLoop()
    {
        // For now we're just gonna loop connected clients (aka humans)
        while (true)
        {
            await Task.Delay(100); // I hate magic numbers, but this stops a deadlock
            
            foreach (var networkConnection in ServerManager.Clients.Values.Where(x => x.FirstObject != null).ToList())
            {
                // Grab the player controller from the network connection and take it's turn
                var playerInstance = networkConnection.FirstObject.GetComponent<LocalPlayerController>();
                await _storyteller.PromptTurn(playerInstance);
            }
        }
    }
}