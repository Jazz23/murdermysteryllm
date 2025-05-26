namespace ArtificialIntelligence.StateMachine;

public abstract class ActionState : State
{
	// Who's taking this action. This reference is useful when queuing and grabbing multiple actions.
	public IPlayer Player { get; init; }
}