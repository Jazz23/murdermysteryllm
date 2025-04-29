using ArtificialIntelligence;
using ArtificialIntelligence.StateMachine;
using UnityEngine;

public class Door : Interactable
{
    [SerializeField]
    private GameObject location;

    public GameObject Location
    {
        get { return location; }
        set { location = value; }
    }


    public override void OnInteraction()
    {
        var player = LocalPlayerController.LocalPlayer;
        // player.StateMachine.QueueAction(new DoorAction { Location = location, Player = player });
    }
}