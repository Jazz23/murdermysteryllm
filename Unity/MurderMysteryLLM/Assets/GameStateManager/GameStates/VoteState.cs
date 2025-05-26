using UnityEngine;
using TMPro;
using System;

public class VoteState : IGameState
{
	private float timeRemaining;
	private bool isVoting = false;
	private TextMeshProUGUI countdownTimerDisplay;
	public async Task OnEnter(GameStateManager gameStateManager)
	{
		// Debug.Log("VoteState: OnEnter");
		isVoting = true;
		this.timeRemaining = gameStateManager.voteTime;
		this.countdownTimerDisplay = gameStateManager.countdownTimerDisplay;
		this.countdownTimerDisplay.text = "";
		countdownTimerDisplay.gameObject.SetActive(true);

		await gameStateManager.PlayTransition("Vote", gameStateManager.transitionTimeVote);
		await StartCountdown(gameStateManager);
	}
	public async Task OnUpdate(GameStateManager gameStateManager)
	{

	}

	public async Task OnExit(GameStateManager gameStateManager)
	{
		// Debug.Log("VoteState: OnExit");
		gameStateManager.countdownTimerDisplay.gameObject.SetActive(false);
		gameStateManager.countdownTimerDisplay.text = "";
		gameStateManager.transitionStateDisplay.text = "";


	}

	private void DisplayTime(float timeToDisplay)
	{
		timeToDisplay += 1f; // To display as countdown
		int minutes = Mathf.FloorToInt(timeToDisplay / 60f);
		int seconds = Mathf.FloorToInt(timeToDisplay % 60f);
		int milliseconds = Mathf.FloorToInt((timeToDisplay % 1f) * 1000f);

		countdownTimerDisplay.text = $"{minutes:00}:{seconds:00}:{milliseconds:00}";
	}

	private async Task StartCountdown(GameStateManager gameStateManager)
	{
		while (isVoting && timeRemaining > 0)
		{
			await Task.Delay(10); // Update every 10 milliseconds
			timeRemaining -= 0.01f; // Decrease time by 10 milliseconds
			DisplayTime(timeRemaining);

			if (timeRemaining <= 0)
			{
				timeRemaining = 0;
				isVoting = false;
			}
		}

		// Transition to the next state when the countdown ends
		if (!isVoting)
		{
			await gameStateManager.ChangeToNextState();
		}
	}
}
