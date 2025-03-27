using System.Threading.Tasks;

public partial class AIAgent
{
    public void TakeDoor(string doorName, string message)
    {
        PlayerInfo.CurrentLocation = doorName;
        var location = AIInterface.Locations.First(x => x.name.ToLower() == doorName.ToLower()).position;
        _navAgent.Warp(location);
        _navAgent.destination = location;
        // TODO: Append the message to some type of context object to be used
    }
}