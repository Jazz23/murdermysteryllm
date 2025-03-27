using FishNet.Object;

public partial class AIAgent
{
    public HashSet<string> CluesFound { get; } = new();

    public void Search(NetworkObject obj)
    {
        CluesFound.Add(obj.GetComponent<Searchable>().Search());
    }
}