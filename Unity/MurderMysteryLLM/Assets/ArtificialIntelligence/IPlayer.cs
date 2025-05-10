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
    
    public void TurnStart();
    
    public void Search(GameObject obj);

    /// <summary>
    /// Send a chat message to another player. Think of the body of this method as "Talked At".
    /// </summary>
    public Task OnTalkedAt(IPlayer other, string message);

    /// <summary>
    /// Initiate a conversation.
    /// </summary>
    public void StartTalking(TalkingAction action);

    public void StopTalking();

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