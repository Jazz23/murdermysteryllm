using System;
using UnityEngine;
using UnityEngine.Rendering;

public class AgentObserver : MonoBehaviour
{
	private const int recordContextDelay = 3000;
	private static int MAX_SEARCHABLE_OBJECT = 20;
    private static int MAX_PEERS_NEARBY = 10;
    private static int MAX_CLUES = 5;

    [SerializeField]
    private List<GameObject> _searchableObject;
    [SerializeField]
    private List<GameObject> _peersNearby;

    [SerializeField]
    private List<GameObject> _locations;

    [SerializeField]
    [TextArea(5, 10)]
    private string context;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Agent") || other.gameObject.name == "Player")
        {
            _peersNearby.Add(other.gameObject);
        }
        else if (other.CompareTag("Searchable") || other.gameObject.layer == LayerMask.NameToLayer("Searchable"))
        {
            _searchableObject.Add(other.gameObject);
        }
        else if (other.CompareTag("Location"))
        {
            _locations.Add(other.gameObject);
        }

    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Agent") || other.gameObject.name == "Player")
        {
            _peersNearby.Remove(other.gameObject);
        }
        else if (other.CompareTag("Searchable"))
        {
            _searchableObject.Remove(other.gameObject);
        }
        else if (other.CompareTag("Location"))
        {
            _locations.Remove(other.gameObject);
        }
    }

    private void Update() {
        
    }

    async private void RecordContext()
    {
        ConvertToContext();
        await Task.Delay(recordContextDelay);
        this.context = "";
    }


     public string ConvertToContext()
    {
        // string context = "";
        string currentLocation = $"Current Location : '{this._locations.First()}' ";
        string searchableObjects = $"Searchable Objects : '{string.Join(", ", this._searchableObject.Select(x => x.name))}' ";
        string peersNearby = $"Peers Nearby : '{string.Join(", ", this._peersNearby.Select(x => x.name))}' ";
        
        string locations = $"Locations : '{string.Join(", ", this._locations.Select(x => x.name))}' ";
        context += currentLocation + "\n" + searchableObjects + "\n" + peersNearby + "\n" + locations + "\n";

        return context;

    }

}
