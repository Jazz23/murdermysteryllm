using System.Threading.Tasks;
using ArtificialIntelligence;
using ArtificialIntelligence.Agent;
using FishNet.Object;
using Unity.Collections;
using UnityEngine;

public class Agent : NetworkBehaviour
{
    public AIAgent AIAgent;

    public override void OnStartServer()
    {
        AIAgent.OnTakeDoor += OnTakeDoor;
    }

    public override void OnStopServer()
    {
        AIAgent.OnTakeDoor -= OnTakeDoor;
    }

    private void OnTakeDoor(string doorName)
    {
        transform.MoveToLocation(doorName);
    }
}