using ArtificialIntelligence;
using ArtificialIntelligence.StateMachine;
using FishNet.Connection;

public class Searchable : Interactable
{
    public string clue = "This is an example clue.";

    public override void OnInteractionServer(NetworkConnection conn)
    {
        conn.GetStateMachine().QueueAction(new SearchAction { Item = NetworkObject, Player = conn.GetPlayer()});
    }
    
    public string Search()
    {
        return clue;
    }
}