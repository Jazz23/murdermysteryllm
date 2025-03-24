using System.Threading.Tasks;
using UnityEngine;

public static class Extensions
{
    public static WaitUntil WaitUntil(this Task task) => new(() => task.IsCompleted);

    public static GameObject GetCurrentLocation(this GameObject go) =>
        AIInterface.Locations.OrderBy(x => Vector2.Distance(go.transform.position, x.transform.position))
            .First()
            .gameObject;
}