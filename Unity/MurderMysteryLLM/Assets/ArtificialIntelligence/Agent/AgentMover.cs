using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Stores a list of locations and has public methods to move the agent to a location.
/// </summary>
public class AgentMover : MonoBehaviour
{
	public static List<GameObject> Locations { get; private set; }
	public static string LocationsAsString => string.Join("\n", Locations.Select(l => l.name));
	public static string LocationCount => Locations.Count.ToString();

	private NavMeshAgent _navAgent;

	public void Awake()
	{
		_navAgent = GetComponent<NavMeshAgent>();
		_navAgent.updateRotation = false;
		_navAgent.updateUpAxis = false;

		if (Locations != null)
			return;

		Locations = new List<GameObject>();
		foreach (Transform location in GameObject.Find("RoomColliders").transform)
		{
			Locations.Add(location.gameObject);
		}
	}

	public void GotoLocation(int locationIndex)
	{
		if (locationIndex < 0 || locationIndex >= Locations.Count)
		{
			Debug.LogError($"Invalid location index: {locationIndex}");
			return;
		}

		var targetLocation = Locations[locationIndex];

		// Use center of the collider as the target position
		var targetPosition = targetLocation.GetComponent<BoxCollider2D>().bounds.center;

		// Move the agent to the target location
		_navAgent.destination = targetPosition;
	}
}