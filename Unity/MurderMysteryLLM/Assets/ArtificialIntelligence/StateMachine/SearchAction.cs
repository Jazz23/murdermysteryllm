using UnityEngine;

namespace ArtificialIntelligence.StateMachine;

public class SearchAction : ActionState
{
    public GameObject Item;
    
    public override void Enter()
    {
        Player.Search(Item);
        StateMachine.SetState(new PickPlayerState());
    }
}