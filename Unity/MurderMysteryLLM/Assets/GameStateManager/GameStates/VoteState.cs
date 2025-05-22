using UnityEngine;
using TMPro;
using System;

public class VoteState : IGameState
{
	private float timeRemaining;
	private bool isVoting = false;
	private TextMeshProUGUI countdownTimerDisplay;
	private GameStateManager gameStateManager;
	public void OnEnter(GameStateManager gameStateManager)
	{
		Debug.Log("VoteState: OnEnter");
		this.gameStateManager = gameStateManager;
		timeRemaining = gameStateManager.voteTime;
		countdownTimerDisplay = gameStateManager.countdownTimerDisplay;
		isVoting = true;
		countdownTimerDisplay.text = "";
		countdownTimerDisplay.gameObject.SetActive(true);
	}
	public void OnUpdate(GameStateManager gameStateManager)
	{
		Debug.Log("VoteState: OnUpdate");
		if (!isVoting)
		{
			gameStateManager.ChangetoNextState();
			return;
		}

		UpdateCountdown();

		if (!isVoting)
			gameStateManager.ChangetoNextState();
		else
			DisplayTime(timeRemaining);
	}

	public void OnExit(GameStateManager gameStateManager)
	{
		Debug.Log("VoteState: OnExit");
		gameStateManager.countdownTimerDisplay.gameObject.SetActive(false);
		gameStateManager.countdownTimerDisplay.text = "";
		gameStateManager.ChangetoNextState();

	}

	private void DisplayTime(float timeToDisplay)
	{
		timeToDisplay += 1f; // To display as countdown
		int minutes = Mathf.FloorToInt(timeToDisplay / 60f);
		int seconds = Mathf.FloorToInt(timeToDisplay % 60f);
		int milliseconds = Mathf.FloorToInt((timeToDisplay % 1f) * 1000f);

		countdownTimerDisplay.text = $"{minutes:00}:{seconds:00}:{milliseconds:00}";
	}
	
		private void UpdateCountdown()
	{
		if (isVoting)
		{
			timeRemaining -= Time.deltaTime;
			if (timeRemaining <= 0)
			{
				timeRemaining = 0;
				isVoting = false;
			}
		}
		else
			isVoting = false;
	}
}
