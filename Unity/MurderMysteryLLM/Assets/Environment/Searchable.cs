using ArtificialIntelligence;
using ArtificialIntelligence.StateMachine;
using FishNet.Connection;

public class Searchable : Interactable
{
    public string clue = "This is an example clue.";

    public override void OnInteraction()
    {
        var player = LocalPlayerController.LocalPlayer;
        player.StateMachine.QueueAction(new SearchAction { Item = NetworkObject, Player = player });
    }
    
    public string Search()
    {
        return clue;
    }
}