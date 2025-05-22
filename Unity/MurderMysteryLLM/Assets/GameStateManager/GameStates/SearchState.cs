using UnityEngine;

public class SearchState : IGameState
{
	public float duration;
	private bool isSearching = false;
	private string countdownText = "";
	public void OnEnter(GameStateManager gameStateManager)
	{
		duration = gameStateManager.searchTime;
		isSearching = true;
		gameStateManager.timerText.text = "Searching for clues...";
		gameStateManager.timerText.gameObject.SetActive(true);
	}


	public void OnUpdate(GameStateManager gameStateManager)
	{

	}

	public void OnExit(GameStateManager gameStateManager)
	{
		isSearching = false;
		gameStateManager.NextState();
		gameStateManager.timerText.gameObject.SetActive(false);
		gameStateManager.timerText.text = "";

	}

	private async void Countdown(float duration)
	{
		if (isSearching) return;

		isSearching = true;
		
	}


}
