using UnityEngine;

public partial class LocalPlayerController
{
    public void TakeDoor(string doorName, string message)
    {
        // TextCommunication.DisplayStorytellerText(Owner, message);
        PlayerInfo.CurrentLocation = doorName;
        var location = AIInterface.Locations.First(x => x.name.ToLower() == doorName.ToLower());
        transform.position = location.position;
        
        // Move the camera to the new location but back it up to its original z location
        var newCameraPos = transform.position;
        newCameraPos.z = Camera.main!.transform.position.z;
        Camera.main!.transform.position = newCameraPos;
    }
}