using System;
using System.Reflection;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Diagnostics.CodeAnalysis;
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
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using System.IO;

public partial class AIAgent : MonoBehaviour, IPlayer
{
	public PlayerInfo PlayerInfo { get; set; }
	public OllamaApiClient ChatClient { get; set; }
	public OllamaApiClient ReasonClient { get; set; }

	private AgentMover _agentMover;

	[SerializeField]
	private SpriteRenderer _spriteRender;
	public SpriteRenderer SpriteRender => _spriteRender;

	[SerializeField]
	public TextMeshProUGUI uiDescriptorText;

	[SerializeField]
	public RawImage speechBubble = null;

	[SerializeField]
	private Animator _animator;


	public void Start()
	{
		_agentMover = GetComponent<AgentMover>();
		_agentObserver = GetComponentInChildren<AgentObserver>();

		_spriteRender = GetComponent<SpriteRenderer>();
		uiDescriptorText = GetComponentInChildren<TextMeshProUGUI>();
		speechBubble = GetComponentInChildren<RawImage>();

	}


	/// <summary>
	/// Prepends the setup prompt and current game state/history to the prompt and sends it to Ollama.
	/// </summary>
	/// <returns>The assistant reply from ChatGPT</returns>
	private async Task<string> Ollama(string userPrompt, bool reason = false, bool includeContext = true)
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

			var stopwatch = Stopwatch.StartNew();

			var response = "";
			var chosenClient = reason ? ReasonClient : ChatClient;
			var modelUsed = reason ? "Deepseek R1" : "Gemma 3B";

			if (reason) Debug.Log("Thinking...");

			await foreach (var stream in chosenClient.ChatAsync(new ChatRequest()
			{
				Messages = context
			}))
			{
				response += stream.Message.Content;
			}

			stopwatch.Stop();

			await Awaitable.MainThreadAsync();

			string cleanedResponse = Regex.Replace(response, "<think>.*?</think>", "", RegexOptions.Singleline)
				.Replace("\n", "").Replace("\r", "").Trim();

			LogOllamaRuntimeToCSV(
				task: reason ? "Reasoning" : "Dialogue",
				responseTimeMs: stopwatch.ElapsedMilliseconds,
				characterCount: cleanedResponse.Length,
				modelUsed: modelUsed,
				includeContext: includeContext
			);

			return cleanedResponse;
		}
		catch (Exception e)
		{
			Debug.LogError(e);
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

			var stopwatchTotal = Stopwatch.StartNew();

			var prompt = string.Format(Prompt.TurnStart, PlayerInfo.CharacterInformation.Name);
			var stopwatchResponse = Stopwatch.StartNew();
			var response = (await Ollama(prompt, true)).ToLower();
			stopwatchResponse.Stop();

			long taskTime = 0;

			if (response.Contains("talk"))
			{
				var stopwatchTask = Stopwatch.StartNew();
				_animator.SetTrigger("Talking");
				await TalkTask();
				stopwatchTask.Stop();
				taskTime = stopwatchTask.ElapsedMilliseconds;
			}
			else if (response.Contains("search"))
			{
				var stopwatchTask = Stopwatch.StartNew();
				await SearchTask();
				stopwatchTask.Stop();
				taskTime = stopwatchTask.ElapsedMilliseconds;
			}
			else if (response.Contains("location"))
			{
				var stopwatchTask = Stopwatch.StartNew();
				await LocationTask();
				stopwatchTask.Stop();
				taskTime = stopwatchTask.ElapsedMilliseconds;
			}

			stopwatchTotal.Stop();

			string taskType = response.Contains("talk") ? "talk"
							  : response.Contains("search") ? "search"
							  : response.Contains("location") ? "location"
							  : "unknown";

			LogRuntimeToCSV(taskType, stopwatchTotal.ElapsedMilliseconds, stopwatchResponse.ElapsedMilliseconds, taskTime);
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
		var response = await Ollama(prompt, true);
		// Extract the number from the text response.
		var location = int.Parse(Regex.Match(response, @"\d+").Value);
		_agentMover.GotoLocation(location);
		AIInterface.TurnStateMachine.SetState(new PickPlayerState());
	}


	private void LogOllamaRuntimeToCSV(string task, long responseTimeMs, int characterCount, string modelUsed, bool includeContext)
	{
		string filePath = Path.Combine(Application.dataPath, "RuntimeLogs/OllamaRuntime.csv");
		bool fileExists = File.Exists(filePath);

		using (StreamWriter writer = new StreamWriter(filePath, append: true))
		{
			if (!fileExists)
			{
				writer.WriteLine("Date,Task,ResponseTime(ms),CharacterCount,ModelUsed,IncludeContext");
			}

			string logLine = $"{DateTime.Now},{task},{responseTimeMs},{characterCount},{modelUsed},{includeContext}";
			writer.WriteLine(logLine);
		}
	}

	private void LogRuntimeToCSV(string taskType, long totalTime, long responseTime, long taskTime)
	{
		string path = Path.Combine(Application.dataPath, "RuntimeLogs/task_response_time.csv");
		bool fileExists = File.Exists(path);
		using (StreamWriter writer = new StreamWriter(path, append: true))
		{
			if (!fileExists)
			{
				writer.WriteLine("Date, Character, TaskType,TotalTime(ms),ResponseTime(ms),TaskTime(ms)");
			}
			string logLine = $"{DateTime.Now},{PlayerInfo.CharacterInformation.Name},{taskType},{totalTime},{responseTime},{taskTime}";
			writer.WriteLine(logLine);
		}
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