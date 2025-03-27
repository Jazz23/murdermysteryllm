using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using ArtificialIntelligence;
using ArtificialIntelligence.Agent;
using ArtificialIntelligence.StateMachine;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine.InputSystem;
using UnityEngine;

public partial class LocalPlayerController : NetworkBehaviour, IPlayer
{
    public static LocalPlayerController LocalPlayer { get; private set; }
    
    public float speed;
    public StateMachine StateMachine { get; set; }
    public HashSet<string> CluesFound { get; } = new();
    public PlayerInfo PlayerInfo
    {
        get => _syncedPlayerInfo.Value;
        private set => _syncedPlayerInfo.Value = value;
    }
    
    private readonly SyncVar<PlayerInfo> _syncedPlayerInfo = new();
    private InputAction _moveAction;
    private Rigidbody2D _rigidBody;

    // I gotta stop with the async stuff
    public override void OnStartServer()
    {
        Task.Run(async () =>
        {
            // SyncVars must be on the main thread
            await Awaitable.MainThreadAsync();
            PlayerInfo = await AIInterface.GetPlayerInfo();
        });
    }

    public override void OnStartClient()
    {
        if (!IsOwner)
            return; // This script is attached to someone else in the lobby, don't control them.

        // We are disabled by default (from the editor) so other players don't get controlled (aka Update on
        // this script only runs for the local player)
        enabled = true;
        LocalPlayer = this;
        _moveAction = InputSystem.actions.FindAction("Move");
        _rigidBody = GetComponent<Rigidbody2D>();
    }
    
    [Client]
    private void Update()
    {
        _rigidBody.linearVelocity = _moveAction.ReadValue<Vector2>() * (Time.deltaTime * speed * 100);
    }
    
    public void TurnStart()
    {
        
    }
}