using System.Threading.Tasks;
using UnityEngine;

public class StartState : IGameState
{
	public async Task OnEnter(GameStateManager gameStateManager)
	{
		// Debug.Log("StartState: OnEnter");
		
		gameStateManager.GenerateClueLists();
		_ = gameStateManager.ChangeToNextState();
	}

	public async Task OnExit(GameStateManager gameStateManager)
	{
		// Debug.Log("StartState: OnExit");
		return;
	}

	public async Task OnUpdate(GameStateManager gameStateManager)
	{
		// Debug.Log("StartState: OnUpdate");
		return;
	}
}
