using UnityEngine;

/// <summary>
/// Must add a collider and set the layer to interactable on the gameobject.
/// </summary>
[RequireComponent(typeof(BoxCollider2D))]
public abstract class Interactable : MonoBehaviour
{
    public string hoverMessage;
    /// <summary>
    /// Client has sent an RPC to the server letting the server know about it's interaction
    /// </summary>
    /// <param name="conn"></param>
    public virtual void OnInteraction() { }
    public virtual void OnHoverNear() { }
    public virtual void OnHoverLeave() { }
}