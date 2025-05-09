using UnityEngine;
using UnityEngine.Rendering;

public class AgentObserver : MonoBehaviour
{
    private static int MAX_SEARCHABLE_OBJECT = 20;
    private static int MAX_PEERS_NEARBY = 10;
    private static int MAX_CLUES = 5;

    [SerializeField]
    private List<GameObject> _searchableObject;
    [SerializeField]
    private List<GameObject> _peersNearby;
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"Is being within range {other.gameObject.name}");
        // if(other.CompareTag("Agent")|| other.gameObject.layer == LayerMask.NameToLayer("Player"))
        // {
        //     _peersNearby.Append(other.gameObject);
        // }
        // else if(other.CompareTag("Searchable") || other.gameObject.layer == LayerMask.NameToLayer("Searchable"))
        // {
        //     _searchableObject.Append(other.gameObject);
        // }

    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log($"Is out of within range {other.gameObject.name}");
        // if (other.CompareTag("Agent") || other.gameObject.layer == LayerMask.NameToLayer("Player"))
        // {
        //     _peersNearby.Remove(other.gameObject);
        // }
        // else if (other.CompareTag("Searchable") || other.gameObject.layer == LayerMask.NameToLayer("Searchable"))
        // {
        //    _searchableObject.Remove(other.gameObject);
        // }
    }


    public string ConvertToContext()
    {
        string context = "";


        return context;
    }
}
