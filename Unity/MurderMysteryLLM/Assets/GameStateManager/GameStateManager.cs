using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    private IGameState currentState;

    [SerializeField]

    
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        currentState.OnUpdate(this);
    }

    public void OnGameStateChanged(IGameState newState)
    {
        if (currentState == null)
            currentState.OnExit(this);
        currentState = newState;
        currentState.OnEnter(this);
    }
}

public interface IGameState
{
    public void OnEnter(GameStateManager gameStateManager);
    public void OnUpdate(GameStateManager gameStateManager);
    public void OnExit(GameStateManager gameStateManager);

}

