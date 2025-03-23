using System.Threading.Tasks;
using ArtificialIntelligence.Agent;
using ArtificialIntelligence.StateMachine;

namespace ArtificialIntelligence;

/// <summary>
/// The interface between the AI Library and whatever application is using it. Inherit from this class to play a character
/// in the game. Both AI and players inherit from this interface.
/// </summary>
public interface IPlayer
{
    public PlayerInfo PlayerInfo { get; }
    public StateMachine.StateMachine StateMachine { get; set; }
    
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
    
    // TODO: Add the rest of actions and their logic
    // https://github.com/Jazz23/murdermysteryllm/issues/12
}

public enum PlayerActions
{
    SEARCH,
    TALK,
    DOOR,
    VOTE
}