using System.Threading.Tasks;
using UnityEngine;

public static class Extensions
{
    public static WaitUntil WaitUntil(this Task task) => new(() => task.IsCompleted);
}