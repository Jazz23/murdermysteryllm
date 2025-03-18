using System;
using System.Threading.Tasks;
using ArtificialIntelligence;
using ArtificialIntelligence.Agent;
using FishNet.Object;
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
    public PlayerInfo PlayerInfo { get; private set; }

    private InputAction _moveAction;
    private Rigidbody2D _rigidBody;

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
        LoadLocations();
        PlayerInfo = await AIInterface.GetPlayerInfo();
    }

    private static void LoadLocations()
    {
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

    public async Task<PlayerActions> TakeTurn(string prompt)
    {
        // TODO: Have actual UI for storyteller. Replace all these Debug.Logs.
        Debug.Log(prompt);

        // Convert the action enum to a list of strings
        var actionList = Enum.GetNames(typeof(PlayerActions)).Select(x => x.ToLower()).ToArray();
        // Ask the user
        var action = await TextInput.PollUser(actionList);
        // Cast the response back into the enum
        return Enum.Parse<PlayerActions>(action.ToUpper());
    }

    public async Task<string> AskDoor(string[] adjacentLocations, string prompt)
    {
        Debug.Log(prompt);
        return await TextInput.PollUser(adjacentLocations);
    }

    public void TakeDoor(string doorName, string message)
    {
        Debug.Log(message);
        
        // Move the player to the new location
        var location = locations.First(x => x.name.ToLower() == doorName.ToLower());
        transform.position = location.position;
        PlayerInfo.CurrentLocation = doorName;
        
        // Move the camera to the new location but back it up to its original z location
        var newCameraPos = transform.position;
        newCameraPos.z = Camera.main!.transform.position.z;
        Camera.main!.transform.position = newCameraPos;
    }
}