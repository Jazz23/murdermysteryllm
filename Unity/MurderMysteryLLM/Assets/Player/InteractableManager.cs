using System;
using FishNet.Object;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class InteractableManager : NetworkBehaviour
{
    public float searchRadius = 1.5f;
    private Interactable _activelyHovered;
    private Collider2D[] colliderResults = new Collider2D[10];
    
    public override void OnStartClient()
    {
        // We only manage the local player's interactables, not others
        if (!IsOwner)
            return;

        enabled = true;
        InputSystem.actions.FindAction("Interact").started += OnInteractKey;
    }

    private void OnInteractKey(InputAction.CallbackContext obj)
    {
        if (!_activelyHovered)
            return;
        
        _activelyHovered.OnInteraction();
    }

    void Update()
    {
        // Hover
        
        
        // Perform a circle raycast around the player, searching for interactables
        Array.Clear(colliderResults, 0, colliderResults.Length);
        Physics2D.OverlapCircleNonAlloc(transform.position, searchRadius, colliderResults, LayerMask.GetMask("Interactable"));
        
        foreach (var hit in colliderResults)
        {
            if (!hit || !hit.TryGetComponent<Interactable>(out var interactable))
                continue;

            // If we've already hit this interactable, no further action is needed.
            if (_activelyHovered) return;
            
            _activelyHovered = interactable;
            TextCommunication.DisplayStorytellerText(interactable.interactMessage);
            interactable.OnHoverNear();

            return;
        }
        
        // If we previously were interacting with something but we are no longer, reset the text
        if (_activelyHovered)
        {
            TextCommunication.DisplayStorytellerText("");
            _activelyHovered.OnHoverLeave();
            _activelyHovered = null;
        }
    }
}