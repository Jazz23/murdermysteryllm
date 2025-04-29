using UnityEngine;

namespace ArtificialIntelligence.StateMachine;

/// <summary>
/// A state machine to take each player's turn.
/// </summary>
public class StateMachine
{
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

    public void QueueAction(ActionState action)
    {
        QueuedActions.Add(action);
    }
    
    public void AddPlayer(IPlayer player) => Players.Add(player);
}

public abstract class State
{
    public StateMachine StateMachine { get; set; }
    public virtual void Enter() { }
    public virtual void Exit() { }
    public virtual void Update() { }
}