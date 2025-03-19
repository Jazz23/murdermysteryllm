using System;
using System.Threading.Tasks;
using ArtificialIntelligence;
using ArtificialIntelligence.Agent;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine.InputSystem;
using UnityEngine;

public class LocalPlayerController : NetworkBehaviour, IPlayer
{
    public static LocalPlayerController LocalPlayer { get; private set; }
    /// <summary>
    /// List of references to the locations the player can move to via the door action.
    /// </summary>
    public static List<Transform> locations;
    
    public float speed;

    public PlayerInfo PlayerInfo
    {
        get => _syncedPlayerInfo.Value;
        private set => _syncedPlayerInfo.Value = value;
    }

    private readonly SyncVar<PlayerInfo> _syncedPlayerInfo = new();
    private InputAction _moveAction;
    private Rigidbody2D _rigidBody;

    public override async void OnStartServer()
    {
        PlayerInfo = await AIInterface.GetPlayerInfo();
    }

    public override async void OnStartClient()
    {
        if (!IsOwner)
            return; // This script is attached to someone else in the lobby, don't control them.

        // We are disabled by default (from the editor) so other players don't get controlled (aka Update on
        // this script only runs for the local player)
        enabled = true;
        LocalPlayer = this;
        _moveAction = InputSystem.actions.FindAction("Move");
        _rigidBody = GetComponent<Rigidbody2D>();
        await LoadLocations();
    }

    private static async Awaitable LoadLocations()
    {
        // Scene objects are synced over the network *after* players spawn in, so we have to wait for the
        // AIInterface empty from our scene before we can continue using it.
        while (AIInterface.StoryContext == null)
            await Task.Delay(100);
        
        // Load the locations from the scene by name
        locations = new List<Transform>();
        foreach (var location in AIInterface.StoryContext.LocationGraph)
        {
            var locationObject = GameObject.Find(location.Name);
            if (locationObject)
                locations.Add(locationObject.transform);
        }
    }

    private void Update()
    {
        _rigidBody.linearVelocity = _moveAction.ReadValue<Vector2>() * (Time.deltaTime * speed * 100);
    }

    [Server]
    public async Task<PlayerActions> TakeTurn(string prompt)
    {
        // TODO: Have actual UI for storyteller. Replace all these Debug.Logs.

        // Convert the action enum to a list of strings
        var actionList = Enum.GetNames(typeof(PlayerActions)).Select(x => x.ToLower()).ToArray();
        var actionListCommaSeparated = string.Join(", ", actionList);
        // Ask the user TODO: Use real prompts here
        var action = await TextCommunication.PollUser(Owner, actionList, $"Reply with either {actionListCommaSeparated}");
        // Cast the response back into the enum
        return Enum.Parse<PlayerActions>(action.ToUpper());
    }

    [Server]
    public async Task<string> AskDoor(string[] adjacentLocations, string prompt)
    {
        return await TextCommunication.PollUser(Owner, adjacentLocations, prompt);
    }

    [Server]
    public void TakeDoor(string doorName, string message)
    {
        TextCommunication.DisplayStorytellerText(Owner, message);
        PlayerInfo.CurrentLocation = doorName;
        TakeDoorLocal(Owner, doorName);
    }

    [TargetRpc]
    private void TakeDoorLocal(NetworkConnection conn, string doorName)
    {
        // Move the player to the new location
        var location = locations.First(x => x.name.ToLower() == doorName.ToLower());
        transform.position = location.position;
        
        // Move the camera to the new location but back it up to its original z location
        var newCameraPos = transform.position;
        newCameraPos.z = Camera.main!.transform.position.z;
        Camera.main!.transform.position = newCameraPos;
    }
}