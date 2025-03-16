using ArtificialIntelligence;
using FishNet.Object;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : NetworkBehaviour
{
    public float speed;
    
    private InputAction _moveAction;
    private Rigidbody2D _rigidBody;

    public override void OnStartClient()
    {
        if (!IsOwner)
            return; // This script is attached to someone else in the lobby, don't control them.

        enabled = true;
        _moveAction = InputSystem.actions.FindAction("Move");
        _rigidBody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        _rigidBody.linearVelocity = _moveAction.ReadValue<Vector2>() * (Time.deltaTime * speed * 100);
    // transform.position += new Vector3(moveVec.x, 0, moveVec.y) * Time.deltaTime;
    }
}