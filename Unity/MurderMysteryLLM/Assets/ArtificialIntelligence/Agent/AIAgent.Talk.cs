﻿using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ArtificialIntelligence;
using ArtificialIntelligence.Agent;
using ArtificialIntelligence.StateMachine;
using JetBrains.Annotations;
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
	public string CurrentConversationAsString => string.Join("\n",
		CurrentConversation.Select(x => $"{x.Speaker.PlayerInfo.CharacterInformation.Name}: {x.Text}"));
	public void InjectTestConversation(List<Statement> conversation) => CurrentConversation = conversation;

	private TalkingAction _talkingAction;

	/// <summary>
	/// LLM summarized conversations.
	/// </summary>
	private List<string> _pastConversationSummaries = new();

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
			   Content = CurrentConversationAsString
		   }
	   };
	}

	[AppendContext]
	private List<Message> AppendConversationSummaries()
	{
		if (_pastConversationSummaries.Count == 0)
			return new List<Message>();

		return _pastConversationSummaries.Select(x => new Message
		{
			Role = ChatRole.System,
			Content = x
		}).ToList();
	}

	/// <summary>
	/// Speaks to another agent. Adds a single spoken statement to each agents' CurrentConversation.
	/// </summary>
	public async Task<Statement> SpeakTo(IPlayer agent)
	{
		// Speak to the agent and append the statement to the respective conversations
		
		// If this is the first statement in the conversation, use the special prompt. Otherwise,
		// simply use the last thing spoken (by the other party) as the prompt.
		var lastStatement = CurrentConversation.LastOrDefault()?.Text ?? "Say something to start the conversation.";
		var prompt = string.Format(Prompt.AgentTalk, agent.PlayerInfo.CharacterInformation.Name, lastStatement);
		
		var completion = await Ollama(prompt);

		// The conversation continues, append the new goodies to our conversation history
		var words = completion;
		var newStatement = new Statement
		{
			Speaker = this,
			Text = words
		};

		if (CurrentConversation.Count > 5)
		{
			return new Statement
			{
				Speaker = this,
				Text = EndConvoKeyword
			};
		}

		CurrentConversation.Add(newStatement);

		Debug.Log($"{PlayerInfo.CharacterInformation.Name}: {words}");
		return newStatement;
	}

	public async Task OnTalkedAt(IPlayer other, string message)
	{
		// Add the message to our conversation
		var statement = new Statement
		{
			Speaker = other,
			Text = message
		};
		CurrentConversation.Add(statement);

		// Respond to the person who talked to us

		var myResponse = await SpeakTo(other);

		// Conversation is over, don't invoke their OnTalkedAt
		if (myResponse.Text == EndConvoKeyword)
		{
			await _talkingAction.EndConversation();
		}
		else
		{
			await other.OnTalkedAt(this, myResponse.Text);
		}
	}

	public void StartTalking(TalkingAction action)
	{
		_talkingAction = action;
		// To begin a conversation, grab a message from the LLM and send it to the other agent
		_ = Task.Run(async () =>
		{
			var text = await SpeakTo(action.Other);
			await action.Other.OnTalkedAt(this, text.Text);
		});
	}

	public async Task StopTalking()
	{
		// Mark that we have begun summarizing. Agent does not take their turn until this is done.
		await SummarizeConversation();
		_talkingAction = null;
	}

	/// <summary>
	/// Summarizes the current conversation and stores it in the _pastConversationSummaries list to be added
	/// as context to future prompts.
	/// </summary>
	public async Task SummarizeConversation()
	{
		var prompt = string.Format(Prompt.SummarizeConversation, CurrentConversationAsString);
		var completion = await Ollama(prompt);
		Debug.Log($"Summary: {completion}");
		_pastConversationSummaries.Add(completion);
		CurrentConversation.Clear();
	}
}