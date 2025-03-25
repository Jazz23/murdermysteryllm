using System.Threading.Tasks;

namespace ArtificialIntelligence.Agent;

public partial class AIAgent
{
    public void TakeDoor(string doorName, string message)
    {
        PlayerInfo.CurrentLocation = doorName;
        OnTakeDoor?.Invoke(doorName);
        // TODO: Append the message to some type of context object to be used
    }
}