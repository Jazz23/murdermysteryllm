using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class TestAgent : MonoBehaviour
{
    private NavMeshAgent _agent;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _agent.updateRotation = false;
        _agent.updateUpAxis = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Get the mouse position in screen space
        var mouseScreenPosition = Input.mousePosition;

        // Convert the screen position to world space
        var mouseWorldPosition = Camera.main!.ScreenToWorldPoint(mouseScreenPosition);

        // Set the z-coordinate to 0 for 2D
        mouseWorldPosition.z = 0;
        _agent.destination = mouseWorldPosition;
    }
}
