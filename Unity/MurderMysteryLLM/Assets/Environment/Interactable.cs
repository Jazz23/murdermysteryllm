using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public string interactMessage;

    /// <summary>
    /// Invoked by InteractionManager when the local player interacts with this object
    /// </summary>
    public virtual void OnInteraction() { }
    public virtual void OnHoverNear() { }
    public virtual void OnHoverLeave() { }
}