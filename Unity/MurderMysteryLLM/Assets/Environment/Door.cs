using ArtificialIntelligence.StateMachine;
using UnityEngine;

public class Door : Interactable
{
    public GameObject location;
    
    public override void OnInteraction()
    {
        LocalPlayerController.LocalPlayer.StateMachine.QueueAction(new DoorAction { Location = location, Player = LocalPlayerController.LocalPlayer });
    }
}