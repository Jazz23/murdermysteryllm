using UnityEngine;

using System.Threading.Tasks;
using UnityEngine;

public class EndState : IGameState
{
	public async Task OnEnter(GameStateManager gameStateManager)
	{
		// Logic for entering the state
		await Task.CompletedTask;
	}

	public async Task OnExit(GameStateManager gameStateManager)
	{
		// Logic for exiting the state
		await Task.CompletedTask;
	}

	public async Task OnUpdate(GameStateManager gameStateManager)
	{
		// Logic for updating the state
		await Task.CompletedTask;
	}
}
