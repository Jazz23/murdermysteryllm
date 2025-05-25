namespace ArtificialIntelligence.StateMachine;

public class WaitForTurnState : State
{
    public override void Update()
    {
        // If there are no queued actions pertaining to the current player, return until one is added
        if (StateMachine.QueuedActions.Find(x => x.Player == StateMachine.CurrentPlayer) 
            is not { } currentPlayerAction)
            return;

		_ = StateMachine.QueuedActions.Remove(currentPlayerAction);
        StateMachine.SetState(currentPlayerAction);
    }
}