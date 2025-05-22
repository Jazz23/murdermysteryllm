using UnityEngine;

public class StartState : IGameState
{
	public void OnEnter(GameStateManager gameStateManager)
	{
		Debug.Log("StartState: OnEnter");

		gameStateManager.FindPlayers();

		int randomIndex = Random.Range(0, gameStateManager.players.Count);
		gameStateManager.killer = gameStateManager.players[randomIndex];

		gameStateManager.GenerateClueLists();

		gameStateManager.ChangetoNextState();
	}

	public void OnExit(GameStateManager gameStateManager)
	{
		Debug.Log("StartState: OnExit");
		return;
	}

	public void OnUpdate(GameStateManager gameStateManager)
	{
		Debug.Log("StartState: OnUpdate");
		return;
	}
}
