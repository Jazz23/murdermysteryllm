using System.Threading.Tasks;
using ArtificialIntelligence.Agent;
using ArtificialIntelligence.StateMachine;
using UnityEngine;

namespace ArtificialIntelligence;

/// <summary>
/// The interface between the AI Library and whatever application is using it. Inherit from this class to play a character
/// in the game. Both AI and players inherit from this interface.
/// </summary>
public interface IPlayer
{
    public PlayerInfo PlayerInfo { get; }
    public HashSet<string> CluesFound { get; }
    public StateMachine.StateMachine StateMachine { get; set; }
    
    public void TurnStart();
    
    public void Search(GameObject obj);
    
    /// <summary>
    /// Invoked by the storyteller after finalizing a new location to be at. This method should move the player.
    /// </summary>
    public void TakeDoor(string doorName, string message);

    public void TalkTo(IPlayer other);

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