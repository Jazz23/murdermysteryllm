using System;
using UnityEngine;
using UnityEngine.Rendering;

public class AgentObserver : MonoBehaviour
{
	public List<GameObject> PeersNearby;
	public List<GameObject> Locations;
	public List<GameObject> SearchableObject;

	private const int recordContextDelay = 3000;
	private static int MAX_SEARCHABLE_OBJECT = 20;
	private static int MAX_PEERS_NEARBY = 10;
	private static int MAX_CLUES = 5;

	[SerializeField]
	[TextArea(5, 10)]
	private string context;

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag("Agent") || other.gameObject.name == "Player")
		{
			PeersNearby.Add(other.gameObject);
		}
		else if (other.CompareTag("Searchable") || other.gameObject.layer == LayerMask.NameToLayer("Searchable"))
		{
			SearchableObject.Add(other.gameObject);
		}
		else if (other.CompareTag("Location"))
		{
			Locations.Add(other.gameObject);
		}

	}

	private void OnTriggerExit2D(Collider2D other)
	{
		if (other.CompareTag("Agent") || other.gameObject.name == "Player")
		{
			_ = PeersNearby.Remove(other.gameObject);
		}
		else if (other.CompareTag("Searchable"))
		{
			_ = SearchableObject.Remove(other.gameObject);
		}
		else if (other.CompareTag("Location"))
		{
			_ = Locations.Remove(other.gameObject);
		}
	}

	private void Update()
	{

	}

	async private void RecordContext()
	{
		_ = ConvertToContext();
		await Task.Delay(recordContextDelay);
		this.context = "";
	}


	public string ConvertToContext()
	{
		string context = "";
		// string currentLocation = $"Current Location : '{this.Locations.First()}' ";
		string searchableObjects = $"Searchable Objects : '{string.Join(", ", this.SearchableObject.Select(x => x.name))}' ";
		string peersNearby = $"Peers Nearby : '{string.Join(", ", this.PeersNearby.Select(x => x.name))}' ";

		string locations = $"Locations : '{string.Join(", ", this.Locations.Select(x => x.name))}' ";
		context += /*currentLocation + "\n" + */ searchableObjects + "\n" + peersNearby + "\n" + locations + "\n";

		return context;

	}

}
