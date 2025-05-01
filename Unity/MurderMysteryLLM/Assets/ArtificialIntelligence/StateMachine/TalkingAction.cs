namespace ArtificialIntelligence.StateMachine;

public class TalkingAction : ActionState
{
    public IPlayer Other { get; init; }

    public override void Enter()
    {
        Player.StartTalking(this);
    }

    public void EndConversation()
    {
        StateMachine.SetState(new PickPlayerState());
    }
}