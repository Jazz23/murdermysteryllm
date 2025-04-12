using System.Collections.Concurrent;
using ArtificialIntelligence.Agent;
using ArtificialIntelligence.StateMachine;
using UnityEngine.InputSystem;
using UnityEngine;

public partial class LocalPlayerController : MonoBehaviour, IPlayer
{
    public static LocalPlayerController LocalPlayer { get; private set; }
    public static bool IsInputBlocked => _blockInputCounter > 0;
    
    public float speed;
    public StateMachine StateMachine { get; set; }
    public HashSet<string> CluesFound { get; } = new();
    public PlayerInfo PlayerInfo { get; private set; }
    
    private InputAction _moveAction;
    private Rigidbody2D _rigidBody;
    
    /// <summary>
    /// When greater than 0, the player cannot move. The reason this is an int is to allow for multiple sources
    /// to block input.
    /// </summary>
    private static int _blockInputCounter = 0;

    /// <summary>
    /// Make sure to call UnblockInput only once after calling this method.
    /// </summary>
    public static void BlockInput() => _blockInputCounter++;
    public static void UnblockInput() => _blockInputCounter--;

    public void Awake()
    {
        LocalPlayer = this;
    }

    // I gotta stop with the async stuff
    public void Start()
    {
        _moveAction = InputSystem.actions.FindAction("Move");
        _rigidBody = GetComponent<Rigidbody2D>();
        
        Task.Run(async () =>
        {
            await Awaitable.MainThreadAsync();
            PlayerInfo = await AIInterface.GetPlayerInfo(0);
        });
    }
    
    private void Update()
    {
        if (IsInputBlocked)
            return;
        
        _rigidBody.linearVelocity = _moveAction.ReadValue<Vector2>() * (Time.deltaTime * speed * 100);
    }
    
    public void TurnStart()
    {
        
    }

    public void OnTalkedAt(IPlayer other, string message)
    {
        Chat.AddChatMessage($"{other.PlayerInfo.CharacterInformation.Name}: {message}");
    }
}