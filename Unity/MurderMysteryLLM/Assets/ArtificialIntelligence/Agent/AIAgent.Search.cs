using FishNet.Object;

namespace ArtificialIntelligence.Agent;

public partial class AIAgent
{
    public HashSet<string> CluesFound { get; } = new();

    public void Search(NetworkObject obj)
    {
        OnSearch?.Invoke(obj);
    }
}