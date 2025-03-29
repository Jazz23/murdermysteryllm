using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class InteractableManager : MonoBehaviour
{
    public float searchRadius = 1.5f;
    private Interactable _activelyHovered;
    private Collider2D[] colliderResults = new Collider2D[10];
    
    public void Start()
    {
        InputSystem.actions.FindAction("Interact").started += OnInteractKey;
    }

    private void OnInteractKey(InputAction.CallbackContext obj)
    {
        _activelyHovered?.OnInteraction();
    }

    void Update()
    {
        // Hover
        
        
        // Perform a circle raycast around the player, searching for interactables
        Array.Clear(colliderResults, 0, colliderResults.Length);
        Physics2D.OverlapCircleNonAlloc(transform.position, searchRadius, colliderResults, LayerMask.GetMask("Interactable"));
        
        // Iterate each collider in order of closest to farthest
        foreach (var hit in colliderResults.Where(x => x).OrderBy(x => Vector3.Distance(x.transform.position, transform.position)))
        {
            if (!hit || !hit.TryGetComponent<Interactable>(out var interactable))
                continue;

            // If we've already hit this interactable, no further action is needed.
            if (_activelyHovered != null) return;
            
            _activelyHovered = interactable;
            TextCommunication.DisplayStorytellerText(interactable.hoverMessage);
            interactable.OnHoverNear();

            return;
        }
        
        // If we previously were interacting with something but we are no longer, reset the text
        if (_activelyHovered != null)
        {
            TextCommunication.DisplayStorytellerText("");
            _activelyHovered.OnHoverLeave();
            _activelyHovered = null;
        }
    }
}