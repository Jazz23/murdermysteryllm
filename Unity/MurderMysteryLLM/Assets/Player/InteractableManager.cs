using System;
using FishNet.Object;
using UnityEngine;
using UnityEngine.EventSystems;

public class InteractableManager : NetworkBehaviour
{
    public float searchRadius = 1.5f;
    private bool _hasHit = false;
    private Collider2D[] colliderResults = new Collider2D[10];
    
    public override void OnStartClient()
    {
        if (IsOwner)
            enabled = true;
    }

    void Update()
    {
        // Perform a circle raycast around the player, searching for interactables
        Array.Clear(colliderResults, 0, colliderResults.Length);
        Physics2D.OverlapCircleNonAlloc(transform.position, searchRadius, colliderResults, LayerMask.GetMask("Interactable"));

        foreach (var hit in colliderResults)
        {
            if (!hit || !hit.TryGetComponent<Interactable>(out var interactable))
                continue;
            
            _hasHit = true;
            TextCommunication.DisplayStorytellerText(interactable.interactMessage);
            return;
        }
        
        // If we previously were interacting with something but we are no longer, reset the text
        if (_hasHit)
        {
            TextCommunication.DisplayStorytellerText("");
        }
    }
}