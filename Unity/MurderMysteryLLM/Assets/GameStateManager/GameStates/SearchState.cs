using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class SearchState : IGameState
{
	private float timeRemaining;
	private bool isSearching = false;
	private TextMeshProUGUI countdownTimerDisplay;

	public async Task OnEnter(GameStateManager gameStateManager)
	{
		isSearching = true;
		this.timeRemaining = gameStateManager.searchTime;
		this.countdownTimerDisplay = gameStateManager.countdownTimerDisplay;
		this.countdownTimerDisplay.text = "";
		countdownTimerDisplay.gameObject.SetActive(true);


		await gameStateManager.PlayTransition("Search");

		// Start the countdown timer asynchronously
		await StartCountdown(gameStateManager);
	}

	public async Task OnUpdate(GameStateManager gameStateManager)
	{
		// No need to handle countdown logic here anymore
	}

	public async Task OnExit(GameStateManager gameStateManager)
	{
		countdownTimerDisplay.gameObject.SetActive(false);
		countdownTimerDisplay.text = "";
		gameStateManager.transitionStateDisplay.text = "";
		await gameStateManager.PlayTransition("Vote");
	}

	private async Task StartCountdown(GameStateManager gameStateManager)
	{
		while (isSearching && timeRemaining > 0)
		{
			await Task.Delay(10); // Update every 10 milliseconds
			timeRemaining -= 0.01f; // Decrease time by 10 milliseconds
			DisplayTime(timeRemaining);

			if (timeRemaining <= 0)
			{
				timeRemaining = 0;
				isSearching = false;
			}
		}

		// Transition to the next state when the countdown ends
		if (!isSearching)
		{
			await gameStateManager.ChangeToNextState();
		}
	}

	private void DisplayTime(float timeToDisplay)
	{
		timeToDisplay += 1f; // To display as countdown
		int minutes = Mathf.FloorToInt(timeToDisplay / 60f);
		int seconds = Mathf.FloorToInt(timeToDisplay % 60f);
		int milliseconds = Mathf.FloorToInt((timeToDisplay % 1f) * 1000f);

		countdownTimerDisplay.text = $"{minutes:00}:{seconds:00}:{milliseconds:00}";
	}
}
