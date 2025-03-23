using System;
using System.Collections;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using ArtificialIntelligence;
using ArtificialIntelligence.Agent;
using ArtificialIntelligence.StateMachine;
using dotenv.net;
using FishNet.Component.Spawning;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Transporting;
using OpenAI.Chat;
using UnityEngine;

/// <summary>
/// Used to interface Unity with AI library. This starts a thread to drive the Storyteller and AIAgents.
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
    /// <summary>
    /// List of references to the locations the player can move to via the door action.
    /// </summary>
    public static List<Transform> Locations;

    public bool mockStoryContext = true;
    public bool mockPlayerInfo = true;
    public int agentCount = 1;
    public GameObject defaultLocation;

    [SerializeField] private NetworkObject _agentPrefab;
    private List<Agent> _agents;
    private Storyteller _storyteller;
    private readonly SyncVar<StoryContext> _syncedStoryContext = new();
    private static AIInterface _instance;

    // We don't want anything to trigger until after initializing our AI Library
    public void Awake() => enabled = false;

    public override void OnStartNetwork()
    {
        // This singleton is only attached to one empty (AIInterface) object at the root of the scene
        _instance = this;
    }
    
    public override void OnStartServer()
    {
        NetworkManager.GetComponent<PlayerSpawner>().OnSpawned += OnPlayerSpawned;
        Task.Run(async () =>
        {
            try
            {
                await Awaitable.MainThreadAsync();
                // Only the server should mess around with the AI library
                await InitAILibrary();
                LoadLocations();
                await SpawnAI();
                await PlayLoop();
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }
        });
    }

    public override void OnStartClient()
    {
        LoadLocations();
    }

    [Server]
    private void OnPlayerSpawned(NetworkObject obj)
    {
        var player = obj.GetComponent<IPlayer>();
        var defaultStateMachine = StateMachine.LocationStateMachines.First(x => x.Location == defaultLocation);
        
        defaultStateMachine.AddPlayer(player);
        player.StateMachine = defaultStateMachine;
    }

    [Server]
    private void FixedUpdate()
    {
        foreach (var sm in StateMachine.LocationStateMachines)
        {
            sm.Update();
        }
    }
    
    private static void LoadLocations()
    {
        if (Locations != null)
            return;
        
        // Load the locations from the scene by name
        Locations = new List<Transform>();
        foreach (var location in StoryContext.LocationGraph)
        {
            var locationObject = GameObject.Find(location.Name);
            if (locationObject)
                Locations.Add(locationObject.transform);
        }
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

        if (mockStoryContext)
        {
            // If we're mocking story context, load from a file instead of generating it
            StoryContext = await Helpers.CreateStoryContextFromJsonFile(
                Path.Combine(promptPathPrefix, "StorytellerPrompts/storyObject.eg.jsonc")); // Temporary: This will be generated via ChatGPT in the future
        }

        _storyteller = new Storyteller();
        StateMachine.LocationStateMachines =
            GameObject.FindGameObjectsWithTag("Location").Select(x => new StateMachine { Location = x }).ToArray();
    }

    [Server]
    private async Awaitable SpawnAI()
    {
        _agents = (await InstantiateAsync(_agentPrefab, agentCount)).Select(x => x.GetComponent<Agent>()).ToList();
        foreach (var agent in _agents)
        {
            agent.AIAgent = new AIAgent(ChatClient, await GetPlayerInfo());
            ServerManager.Spawn(agent.NetworkObject);
        }
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
        enabled = true;
        
        // // For now we're just gonna loop connected clients (aka humans)
        // while (true)
        // {
        //     await Task.Delay(100); // I hate magic numbers, but this stops a deadlock
        //     
        //     foreach (var networkConnection in ServerManager.Clients.Values.Where(x => x.FirstObject != null).ToList())
        //     {
        //         // Grab the player controller from the network connection and take it's turn
        //         var playerInstance = networkConnection.FirstObject.GetComponent<LocalPlayerController>();
        //         await _storyteller.PromptTurn(playerInstance);
        //     }
        //
        //     foreach (var agent in _agents)
        //     {
        //         await _storyteller.PromptTurn(agent.AIAgent);
        //     }
        // }
    }
}