using System.Linq;
using UnityEngine;

/// <summary>
/// This class will be shared between AI and real players. This is what interfaces with the storyteller from the
/// AI library.
/// </summary>
public class Player : MonoBehaviour
{
    /// <summary>
    /// List of references to the locations the player can move to via the door action.
    /// </summary>
    public static Transform[] locations;
    
    public void Awake()
    {
        // If another player has already loaded the static references, return
        if (locations != null)
            return;
        
        
    }
    
    #region Actions

    /// <summary>
    /// Moves the player to specified location and returns their new position.
    /// </summary>
    public Vector3 PerformDoorAction(string locationName)
    {
        var location = locations.First(x => x.name.ToLower() == locationName.ToLower());
        transform.position = location.position;
        return transform.position;
    }
    
    
    #endregion
}