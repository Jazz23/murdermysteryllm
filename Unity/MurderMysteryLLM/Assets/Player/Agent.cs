
using System.Runtime.CompilerServices;
using ArtificialIntelligence.Agent;
using ArtificialIntelligence.StateMachine;
using FishNet.Object;
using UnityEngine;
using UnityEngine.AI;

public class Agent : NetworkBehaviour
{
    public AIAgent AIAgent;
    private NavMeshAgent _navAgent;

    public override void OnStartServer()
    {
        AIAgent.OnTakeDoor += OnTakeDoor;
        AIAgent.OnTurnStart += OnTurnStart;
        AIAgent.OnSearch += OnSearch;
        _navAgent = GetComponent<NavMeshAgent>();
        _navAgent.updateRotation = false;
        _navAgent.updateUpAxis = false;
    }

    private void OnSearch(NetworkObject obj)
    {
        AIAgent.CluesFound.Add(obj.GetComponent<Searchable>().Search());
    }

    private void OnTurnStart()
    {
        
        // For now, just pick the nearest door and go to it
        var targetDoor = gameObject.GetCurrentLocation().GetComponentsInChildren<Door>()
            .First();
        
        _navAgent.destination = targetDoor.transform.position;
        
        Task.Run(async () =>
        {
            await Awaitable.MainThreadAsync();
            await Awaitable.NextFrameAsync(); // Make sure we start moving
            while (_navAgent.velocity != Vector3.zero)
                await Awaitable.NextFrameAsync();
            
            AIAgent.StateMachine.QueueAction(new DoorAction { Location = targetDoor.location, Player = AIAgent });
        });
    }

    public override void OnStopServer()
    {
        AIAgent.OnTakeDoor -= OnTakeDoor;
    }

    private void OnTakeDoor(string doorName)
    {
        var location = AIInterface.Locations.First(x => x.name.ToLower() == doorName.ToLower()).position;
        _navAgent.Warp(location);
        _navAgent.destination = location;
    }
}