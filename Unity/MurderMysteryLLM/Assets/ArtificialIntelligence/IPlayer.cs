using System.Threading.Tasks;
using ArtificialIntelligence.Agent;

namespace ArtificialIntelligence;

public interface IPlayer
{
    public PlayerInfo PlayerInfo { get; }
    
    /// <summary>
    /// Given a string message, returns an action to be taken.
    /// </summary>
    public Task<PlayerActions> TakeTurn(string prompt);
    
    /// <summary>
    /// "Which door would you like to enter?" Returns the name of the desired location.
    /// </summary>
    public Task<string> AskDoor(string[] adjacentLocations, string prompt);

    /// <summary>
    /// Invoked by the storyteller after finalizing a new location to be at. This method should move the player.
    /// </summary>
    public void TakeDoor(string doorName, string message);
}

public enum PlayerActions
{
    SEARCH,
    TALK,
    DOOR,
    VOTE
}