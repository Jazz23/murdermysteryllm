using System;
using System.Collections;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using ArtificialIntelligence;
using ArtificialIntelligence.Agent;
using ArtificialIntelligence.StateMachine;
using dotenv.net;
using OpenAI.Chat;
using UnityEngine;

/// <summary>
/// Used to interface Unity with AI library. This starts a thread to drive the Storyteller and AIAgents.
/// </summary>
public class AIInterface : MonoBehaviour
{
    // Synchronize the server's StoryContext to the rest of the clients
    public static StoryContext StoryContext { get; private set; }
    public static ChatClient ChatClient;
    /// <summary>
    /// List of references to the locations the player can move to via the door action.
    /// </summary>
    public static List<Transform> Locations;

    public bool mockStoryContext = true;
    public bool mockPlayerInfo = true;
    public int agentCount = 1;
    public GameObject defaultLocation;
    private static StateMachine DefaultStateMachine =>
        StateMachine.LocationStateMachines.First(x => x.Location == _instance.defaultLocation);

    [SerializeField] private GameObject _agentPrefab;
    private List<AIAgent> _agents;
    private Storyteller _storyteller;
    private readonly StoryContext _syncedStoryContext = new();
    private static AIInterface _instance;

    public void Awake()
    {
        // We don't want anything to trigger until after initializing our AI Library
        enabled = false;
        _instance = this;

        Task.Run(async () =>
        {
            try
            {
                await Awaitable.MainThreadAsync();
                await SpawnAI();
                // Only the server should mess around with the AI library
                await InitAILibrary();
                LoadLocations();
                await PlayLoop();
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }
        });
    }

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

        var player = LocalPlayerController.LocalPlayer.GetComponent<IPlayer>();
        player.StateMachine = DefaultStateMachine;
        DefaultStateMachine.AddPlayer(player);
    }

    private async Awaitable SpawnAI()
    {
        _agents = (await InstantiateAsync(_agentPrefab, agentCount)).Select(x => x.GetComponent<AIAgent>()).ToList();

        foreach (var agent in _agents)
        {
            agent.ChatClient = ChatClient;
            agent.PlayerInfo = await GetPlayerInfo(1);
            // agent.StateMachine = DefaultStateMachine;
            // DefaultStateMachine.AddPlayer(agent);
        }
    }

    public static async Awaitable<PlayerInfo> GetPlayerInfo(int index)
    {
        // if (_instance.MockPlayerInfo)
        // {
        var promptPathPrefix = Environment.GetEnvironmentVariable("PROMPTS_PATH")!;
        return await Helpers.GetPlayerInfoFromJsonFile(promptPathPrefix + $"AgentPrompts/ExampleData/character{index}.jsonc", "Grand Library",
            StoryContext);
        // }
    }

    /// <summary>
    /// Loops between players and takes their turn via the storyteller.
    /// </summary>
    /// <returns></returns>
    private async Awaitable PlayLoop()
    {
        enabled = true;
    }
}