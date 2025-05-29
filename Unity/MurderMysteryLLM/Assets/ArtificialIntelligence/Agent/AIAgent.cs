using System;
using System.Reflection;
using System.Text.Json;
using ArtificialIntelligence.Agent;
using ArtificialIntelligence.StateMachine;
using OllamaSharp;
using OllamaSharp.Models.Chat;
using OpenAI.Chat;
using TMPro;
using Unity.VisualScripting;
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
	public SpriteRenderer SpriteRender => _spriteRender;

	[SerializeField]
	public TextMeshProUGUI uiDescriptorText;

	[SerializeField]
	public RawImage speechBubble = null;


	public void Start()
	{
		_agentMover = GetComponent<AgentMover>();
		_agentObserver = GetComponentInChildren<AgentObserver>();

		_spriteRender = GetComponent<SpriteRenderer>();
		uiDescriptorText = GetComponentInChildren<TextMeshProUGUI>();
		speechBubble = GetComponentInChildren<RawImage>();
		speechBubble.enabled = false;

	}


	/// <summary>
	/// Prepends the setup prompt and current game state/history to the prompt and sends it to OpenAI.
	/// </summary>
	/// <returns>The assistant reply from ChatGPT</returns>
	private async Task<string> Ollama(string userPrompt, bool includeContext = true)
	{
		try
		{
			await Awaitable.MainThreadAsync();
			var context = includeContext ? BuildChatGPTContext() : new List<Message>();
			context.Add(new Message()
			{
				Role = ChatRole.User,
				Content = userPrompt
			});

			await Awaitable.BackgroundThreadAsync();
			var response = "";
			await foreach (var stream in ChatClient.ChatAsync(new ChatRequest()
			{
				Messages = context
			}))
				response += stream.Message.Content;
			await Awaitable.MainThreadAsync();
			return response.Replace("\n", "").Replace("\r", "").Trim();
		}
		catch (Exception e)
		{
			UnityEngine.Debug.LogError(e);
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
		_ = Task.Run(async () =>
		{
			await Awaitable.MainThreadAsync();

			// var agentToTalkTo = AIInterface.Agents.First(x => x != this);
			// AIInterface.TurnStateMachine.QueueAction(new TalkingAction()
			// {
			//     Other = agentToTalkTo,
			//     Player = this,
			// });

			var prompt = string.Format(Prompt.TurnStart, PlayerInfo.CharacterInformation.Name);
			var response = (await Ollama(prompt)).ToLower();
			Debug.Log($"{PlayerInfo.CharacterInformation.Name} chooses {response}");

			if (response.Contains("talk"))
			{
				await TalkTask();
			}
			else if (response.Contains("search"))
			{
				await SearchTask();
			}
			else if (response.Contains("location"))
			{
				await LocationTask();
			}
		});
	}

	private async Task TalkTask()
	{
		var playerList = _agentObserver.PeersNearby.Select(x => x.GetComponent<IPlayer>()).ToList();
		var playerNames = string.Join(", ", playerList.Select(x => x.PlayerInfo.CharacterInformation.Name));
		var prompt = string.Format(Prompt.PickWhoToTalkTo, playerNames);
		var response = await Ollama(prompt);

		var person = playerList[int.Parse(response)];
		AIInterface.TurnStateMachine.QueueAction(new TalkingAction()
		{
			Other = person,
			Player = this
		});

		// To display the chat box
		if (person is LocalPlayerController)
		{
			var mockTalkingAction = new TalkingAction()
			{
				Other = this,
				Player = person,
				StateMachine = AIInterface.TurnStateMachine
			};
			person.StartTalking(mockTalkingAction);
		}
	}

	private async Task SearchTask()
	{
		AIInterface.TurnStateMachine.SetState(new PickPlayerState());
	}

	private async Task LocationTask()
	{
		var prompt = string.Format(Prompt.LocationQuery, AgentMover.Locations.Count, AgentMover.LocationsAsString);
		var response = await Ollama(prompt);
		// Remove whitespace and newlines
		var location = int.Parse(response);
		_agentMover.GotoLocation(location);
		AIInterface.TurnStateMachine.SetState(new PickPlayerState());
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