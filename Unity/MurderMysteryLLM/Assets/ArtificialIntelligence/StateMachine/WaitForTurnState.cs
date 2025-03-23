namespace ArtificialIntelligence.StateMachine;

public class WaitForTurnState : State
{
    public override void Update()
    {
        if (StateMachine.QueuedActions.Find(x => x.Player == StateMachine.CurrentPlayer) 
            is not { } currentPlayerAction)
            return;

        StateMachine.QueuedActions.Remove(currentPlayerAction);
        StateMachine.SetState(currentPlayerAction);
    }
}