namespace ArtificialIntelligence.StateMachine;

public class TalkingAction : ActionState
{
	public IPlayer Other { get; init; }

	public override void Enter()
	{
		Player.StartTalking(this);
	}

	public async Task EndConversation()
	{
		await Other.StopTalking();
		await Player.StopTalking();

		StateMachine.SetState(new PickPlayerState());
	}
}