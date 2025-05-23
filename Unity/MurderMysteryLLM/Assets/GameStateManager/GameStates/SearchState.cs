using TMPro;
using UnityEngine;

public class SearchState : IGameState
{
	private float timeRemaining;
	private bool isSearching = false;
	private TextMeshProUGUI countdownTimerDisplay;

	public void OnEnter(GameStateManager gameStateManager)
	{
		Debug.Log("SearchState: OnEnter");
		gameStateManager.currentStateDisplay.text = "Search";
		gameStateManager.transitionAnimator.SetBool("startTransition", true);
		timeRemaining = gameStateManager.searchTime;
		countdownTimerDisplay = gameStateManager.countdownTimerDisplay;
		isSearching = true;
		countdownTimerDisplay.text = "";
		countdownTimerDisplay.gameObject.SetActive(true);

		System.Collections.IEnumerator WaitForTransition()
		{
			yield return new WaitForSeconds(1f);
			gameStateManager.transitionAnimator.SetBool("startTransition", false);
		}
	}

	public void OnUpdate(GameStateManager gameStateManager)
	{
		Debug.Log("SearchState: OnUpdate");
		if (!isSearching)
		{
			gameStateManager.ChangetoNextState();
			return;
		}

		UpdateCountdown();

		if (!isSearching)
			gameStateManager.ChangetoNextState();
		else
			DisplayTime(timeRemaining);
	}

	public void OnExit(GameStateManager gameStateManager)
	{
		Debug.Log("SearchState: OnExit");
		countdownTimerDisplay.gameObject.SetActive(false);
		countdownTimerDisplay.text = "";
	}

	private void UpdateCountdown()
	{
		if (isSearching)
		{
			timeRemaining -= Time.deltaTime;
			if (timeRemaining <= 0)
			{
				timeRemaining = 0;
				isSearching = false;
			}
		}
		else
			isSearching = false;

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
