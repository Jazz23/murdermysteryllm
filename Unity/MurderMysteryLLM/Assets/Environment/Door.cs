using ArtificialIntelligence;
using ArtificialIntelligence.StateMachine;
using FishNet.Connection;
using FishNet.Object;
using UnityEngine;

public class Door : Interactable
{
    public GameObject location;
    
    public override void OnInteraction()
    {
        ClientInteracted();
    }

    [ServerRpc(RequireOwnership = false)]
    private void ClientInteracted(NetworkConnection conn = null)
    {
        var player = conn!.FirstObject.GetComponentInChildren<IPlayer>();
        player.StateMachine.QueueAction(new DoorAction { Location = location, Player = player });
    }
}