using System.Threading.Tasks;

namespace ArtificialIntelligence.Agent;

public partial class AIAgent
{
    public async Task<string> AskDoor(string[] adjacentLocations, string prompt)
    {
        // TODO: Use ChatGPT tooling to choose a location
        var chosenDoor = adjacentLocations.First();
        OnAskDoor?.Invoke(chosenDoor);
        return chosenDoor;
    }

    public void TakeDoor(string doorName, string message)
    {
        PlayerInfo.CurrentLocation = doorName;
        OnTakeDoor?.Invoke(doorName);
        // TODO: Append the message to some type of context object to be used
    }
}