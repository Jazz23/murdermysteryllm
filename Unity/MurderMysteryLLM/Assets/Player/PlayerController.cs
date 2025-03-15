using FishNet.Object;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : NetworkBehaviour
{
    private InputAction _moveAction;
    private CharacterController _characterController;
    private float _speed = 5f;

    public override void OnStartClient()
    {
        if (!IsOwner)
            return; // This script is attached to someone else in the lobby, don't control them.

        enabled = true;
        _moveAction = InputSystem.actions.FindAction("Move");
        _characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        var moveVec = _moveAction.ReadValue<Vector2>();
        if (moveVec.sqrMagnitude <= 0.01f)
            return;

        _characterController.Move(new Vector2(moveVec.x, moveVec.y) * (Time.deltaTime * _speed));
        // transform.position += new Vector3(moveVec.x, 0, moveVec.y) * Time.deltaTime;
    }
}