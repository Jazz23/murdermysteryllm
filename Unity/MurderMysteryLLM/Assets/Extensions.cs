using System.Threading.Tasks;
using UnityEngine;

public static class Extensions
{
    public static WaitUntil WaitUntil(this Task task) => new(() => task.IsCompleted);

    public static void MoveToLocation(this Transform transform, string doorName)
    {
        var location = AIInterface.Locations.First(x => x.name.ToLower() == doorName.ToLower());
        transform.position = location.position;
    }
}