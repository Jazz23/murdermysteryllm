using ArtificialIntelligence.Agent;
using ArtificialIntelligence.StateMachine;
using UnityEngine.InputSystem;
using UnityEngine;

public partial class LocalPlayerController : MonoBehaviour, IPlayer
{
    public static LocalPlayerController LocalPlayer { get; private set; }
    
    public float speed;
    public StateMachine StateMachine { get; set; }
    public HashSet<string> CluesFound { get; } = new();
    public PlayerInfo PlayerInfo { get; private set; }
    
    private InputAction _moveAction;
    private Rigidbody2D _rigidBody;

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
            // SyncVars must be on the main thread
            await Awaitable.MainThreadAsync();
            PlayerInfo = await AIInterface.GetPlayerInfo();
        });
    }
    
    private void Update()
    {
        _rigidBody.linearVelocity = _moveAction.ReadValue<Vector2>() * (Time.deltaTime * speed * 100);
    }
    
    public void TurnStart()
    {
        
    }

    public void TalkTo(IPlayer other)
    {
        throw new System.NotImplementedException();
    }
}