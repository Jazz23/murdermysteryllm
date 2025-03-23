using UnityEngine;

namespace ArtificialIntelligence.StateMachine;

public class DoorAction : ActionState
{
    public GameObject Location { get; init; }

    public override void Enter()
    {
        // Add to the new location's state machine
        var newStateMachine = StateMachine.LocationStateMachines.First(x => x.Location == Location);
        StateMachine.SwitchPlayerToStateMachine(Player, newStateMachine);
        
        // Teleport to the new location
        Player.TakeDoor(Location.name, $"You have entered the {Location.name}.");
        
        StateMachine.SetState(new PickPlayerState());
    }
}