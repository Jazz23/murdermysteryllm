using ArtificialIntelligence;
using ArtificialIntelligence.StateMachine;
using UnityEngine;

public class Door : Interactable
{
    public GameObject location;
    
    public override void OnInteraction()
    {
        var player = LocalPlayerController.LocalPlayer;
        player.StateMachine.QueueAction(new DoorAction { Location = location, Player = player });
    }
}