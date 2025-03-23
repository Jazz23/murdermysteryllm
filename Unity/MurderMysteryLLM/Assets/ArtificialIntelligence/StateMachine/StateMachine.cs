using UnityEngine;

namespace ArtificialIntelligence.StateMachine;

/// <summary>
/// A state machine to take each player's turn.
/// </summary>
public class StateMachine
{
    public static StateMachine[] LocationStateMachines;
    
    public GameObject Location { get; init; }
    public readonly List<IPlayer> Players = new();
    public IPlayer CurrentPlayer; // Who's turn it is
    
    /// <summary>
    /// List of actions that have been requested by players.
    /// </summary>
    public List<ActionState> QueuedActions = new();
    
    private State _currentState;

    public StateMachine() => SetState(new PickPlayerState());

    public void Update() => _currentState.Update();

    public void SetState(State newState)
    {
        _currentState?.Exit();
        _currentState = newState;
        _currentState.StateMachine = this;
        _currentState.Enter();
    }

    public void QueueAction(ActionState action) => QueuedActions.Add(action);
    
    public void AddPlayer(IPlayer player) => Players.Add(player);

    public void SwitchPlayerToStateMachine(IPlayer player, StateMachine other)
    {
        // Add to the new location's state machine
        Players.Remove(player);
        other.Players.Add(player);
        player.StateMachine = other;
        if (CurrentPlayer == player)
            CurrentPlayer = null;
    }
}

public abstract class State
{
    public StateMachine StateMachine { get; set; }
    public virtual void Enter() { }
    public virtual void Exit() { }
    public virtual void Update() { }
}