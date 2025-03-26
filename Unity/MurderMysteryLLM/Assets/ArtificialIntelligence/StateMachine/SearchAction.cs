using FishNet.Object;

namespace ArtificialIntelligence.StateMachine;

public class SearchAction : ActionState
{
    public NetworkObject Item;
    
    public override void Enter()
    {
        Player.Search(Item);
        StateMachine.SetState(new PickPlayerState());
    }
}