using System;
using System.Threading.Tasks;
using ArtificialIntelligence;
using ArtificialIntelligence.StateMachine;
using FishNet.Connection;
using FishNet.Object;
using Unity.VisualScripting;
using UnityEngine;

public class BreakableItem : Interactable
{
    [SerializeField] 
    private GameObject location;

    [SerializeField]
    private int breakLimit = 3;
    [SerializeField]
    private int timesInteracted = 0;

    [SerializeField]
    private Animator _animator;

    private void Awake()
    {
        timesInteracted = 0;
        _animator = GetComponentInParent<Animator>();
    }

    public override void OnInteraction()
    {
       timesInteracted += 1;
      Debug.Log("Interaction count: " + timesInteracted);

        if (timesInteracted >= breakLimit)
        {
            OnDestroy();
        }

    }


    async private void OnDestroy() {
        _animator.SetBool("Break", true);
        Destroy(this);
    }







}
