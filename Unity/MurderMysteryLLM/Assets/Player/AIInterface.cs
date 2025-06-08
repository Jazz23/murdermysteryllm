using System;
using System.Collections;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using ArtificialIntelligence;
using ArtificialIntelligence.Agent;
using ArtificialIntelligence.StateMachine;
using dotenv.net;
using OllamaSharp;
using OpenAI.Chat;
using UnityEngine;

/// <summary>
/// Used to interface Unity with AI library. This starts a thread to drive the Storyteller and AIAgents.
/// </summary>
public class AIInterface : MonoBehaviour
{
	// Synchronize the server's StoryContext to the rest of the clients
	public static StoryContext StoryContext { get; private set; }
	public static OllamaApiClient ChatClient;
	public static OllamaApiClient ReasonClient;
	/// <summary>
	/// List of references to the locations the player can move to via the door action.
	/// </summary>
	public static List<Transform> Locations;
	public static List<AIAgent> Agents { get; } = new();
	public static StateMachine TurnStateMachine { get; } = new();
    public bool accessGameStateManager { get; private set; }

    public bool mockStoryContext = true;
	public bool mockPlayerInfo = true;

	[Range(2, 5)]
	public int agentCount = 1;
	public List<Transform> agentSpawnLocations = new();

	[SerializeField] private GameObject _agentPrefab;
	private Storyteller _storyteller;
	private readonly StoryContext _syncedStoryContext = new();
	private static AIInterface _instance;

	public async void Awake()
	{
		// We don't want anything to trigger until after initializing our AI Library
		enabled = false;
		_instance = this;
		GetAgentSpawnLocations();

		await Awaitable.MainThreadAsync();
		await InitAILibrary();
		// await GenertateSceneario();
		await SpawnAI();
		await InitializePlayerAgents();
		await PlayLoop();
	}

	private void FixedUpdate()
	{
		TurnStateMachine.Update();
	}

	// Loads prompts and any testing data
	private async Awaitable InitAILibrary()
	{
		// Place .env in root of unity project
		DotEnv.Load();

		var uri = new Uri("http://localhost:11434");
		ChatClient = new OllamaApiClient(uri);
		ChatClient.SelectedModel = "gemma3";
		
		ReasonClient = new OllamaApiClient(uri);
		ReasonClient.SelectedModel = "deepseek-r1:latest";

		// ChatClient = new ChatClient("gpt-4o-mini", Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

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

		if (!LocalPlayerController.LocalPlayer) return; // Bandaid for some weird bug with the editor
		var player = LocalPlayerController.LocalPlayer.GetComponent<IPlayer>();
		TurnStateMachine.AddPlayer(player);
	}

	private async Awaitable GenerateScenario()
	{
		// generate character sheets 

		// who was murdered and who is killer

		// generate clues 
		

	}

	private async Awaitable SpawnAI()
	{
		for (var i = 0; i < agentCount; i++)
		{
			// Randomly pick a spawn location from the available locations
			if (agentSpawnLocations.Count == 0)
			{
				Debug.LogError("No spawn locations available for agents.");
				return;
			}

			var randomIndex = UnityEngine.Random.Range(0, agentSpawnLocations.Count);
			var selectedSpawnLocation = agentSpawnLocations[randomIndex];
			agentSpawnLocations.RemoveAt(randomIndex); // Remove the selected location from the list

			// Randomly spawn the agent near the selected spawn location
			// var newPos = selectedSpawnLocation.position +
			//              new Vector3(UnityEngine.Random.Range(-2f, 2f), 0, UnityEngine.Random.Range(-2f, 2f));
			Agents.Add(Instantiate(_agentPrefab, selectedSpawnLocation.position, selectedSpawnLocation.rotation).GetComponent<AIAgent>());
		}



	}

	private static async Task InitializePlayerAgents()
	{
		// Create a list of unique indices to assign to agents
		List<int> availableIndices = Enumerable.Range(1, 5).ToList(); // 1 to 5 inclusive

		foreach (var agent in Agents)
		{
			agent.ChatClient = ChatClient;
			agent.ReasonClient = ReasonClient;

			int selectedIndex;
			bool flowControl = FetchRandomAgentIndex(availableIndices, out selectedIndex);
			if (!flowControl) return;

			agent.PlayerInfo = await GetPlayerInfo(selectedIndex);
			agent.uiDescriptorText.text = $"{agent.PlayerInfo.CharacterInformation.Name}\n {agent.PlayerInfo.CharacterInformation.Age}";
			agent.SpriteRender.color = new Color(UnityEngine.Random.Range(0, 1f), UnityEngine.Random.Range(0, 1f), UnityEngine.Random.Range(0, 1f));
			TurnStateMachine.AddPlayer(agent);
		}
		// if(accessGameStateManager)
		// 	GameStateManager.Instance.RetrievePlayerAgents(Agents.Select(agent => agent.gameObject).ToList());
	}

	private static bool FetchRandomAgentIndex(List<int> availableIndices, out int selectedIndex)
	{
		if (availableIndices.Count == 0)
		{
			Debug.LogError("No more unique indices available for agents.");
			selectedIndex = 0;
			return false;
		}

		// Randomly pick an index from the available indices
		int randomIndex = UnityEngine.Random.Range(0, availableIndices.Count);
		selectedIndex = availableIndices[randomIndex];
		availableIndices.RemoveAt(randomIndex); // Remove the selected index from the list
		return true;
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



	private void GetAgentSpawnLocations()
	{
		this.agentSpawnLocations = new List<Transform>();
		Transform spawn = transform.Find("Spawn");
		if (spawn == null) return;
		foreach (Transform child in spawn)
			this.agentSpawnLocations.Add(child);
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