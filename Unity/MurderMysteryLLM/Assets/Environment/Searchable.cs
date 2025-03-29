using ArtificialIntelligence;
using ArtificialIntelligence.StateMachine;

public class Searchable : Interactable
{
    public string clue = "This is an example clue.";

    public override void OnInteraction()
    {
        var player = LocalPlayerController.LocalPlayer;
        player.StateMachine.QueueAction(new SearchAction { Item = gameObject, Player = player });
    }
    
    public string Search()
    {
        return clue;
    }
}