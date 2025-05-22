using UnityEngine;

public class StartState : IGameState
{
	public void OnEnter(GameStateManager gameStateManager)
	{
		gameStateManager.FindPlayers();

		int randomIndex = Random.Range(0, gameStateManager.players.Count);
		gameStateManager.killer = gameStateManager.players[randomIndex];

		gameStateManager.GenerateClueLists();

		gameStateManager.NextState();
	}

	public void OnExit(GameStateManager gameStateManager)
	{
		return;
	}

	public void OnUpdate(GameStateManager gameStateManager)
	{
		return;
	}
}
