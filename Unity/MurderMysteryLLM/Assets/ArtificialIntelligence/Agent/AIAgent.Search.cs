using UnityEngine;

public partial class AIAgent
{
    public HashSet<string> CluesFound { get; } = new();

    public void Search(GameObject obj)
    {
        CluesFound.Add(obj.GetComponent<Searchable>().Search());
    }
}