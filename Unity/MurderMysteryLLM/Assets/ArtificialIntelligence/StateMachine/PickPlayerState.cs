namespace ArtificialIntelligence.StateMachine;

public class PickPlayerState : State
{
	// As soon as we enter this state, pick the next player and transition to a wait for turn state
	public override void Enter()
	{
		if (StateMachine.Players.Count == 0)
			return;

		// Grab the first player in the list and move them to the back of the list
		var player = StateMachine.Players.First();
		StateMachine.Players.RemoveAt(0);
		StateMachine.Players.Add(player);

		StateMachine.CurrentPlayer = player;
		player.TurnStart();
		StateMachine.SetState(new WaitForTurnState());
	}

	public override void Update()
	{
		// While the room is empty, keep checking to see if someone enters
		Enter();
	}
}