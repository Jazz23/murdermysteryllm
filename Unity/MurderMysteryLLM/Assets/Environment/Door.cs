using ArtificialIntelligence;
using ArtificialIntelligence.StateMachine;
using FishNet.Connection;
using FishNet.Object;
using UnityEngine;

public class Door : Interactable
{
    public GameObject location;
    
    public override void OnInteractionServer(NetworkConnection conn)
    {
        var player = conn!.FirstObject.GetComponentInChildren<IPlayer>();
        player.StateMachine.QueueAction(new DoorAction { Location = location, Player = player });
    }
}