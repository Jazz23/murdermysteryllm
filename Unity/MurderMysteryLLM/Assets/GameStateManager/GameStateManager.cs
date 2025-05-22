using TMPro;
using UnityEditor;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UI;


public class GameStateManager : MonoBehaviour
{

    [Header("GameStates")]
    public IGameState currentState;
    [Tooltip("Time in seconds to search for clues")]
    [Range(0,180)]
    public float searchTime = 75f;

    [Tooltip("Time in seconds to vote")]
    [Range(0, 180)]
    public float voteTime = 75f;
    public StartState startState = new StartState();
    public SearchState searchState = new SearchState();
    public VoteState voteState = new VoteState();
    public EndState endState = new EndState();

    [Header("Scene")]
    public List<GameObject> players;
    public GameObject killer;


    [Header("Clues")]

    [Range(0, 14)]
    [SerializeField]
    public int realCluesNecessary = 10;

    [SerializeField]
    public List<GameObject> clues;

    [SerializeField]
    public List<GameObject> fakeClues;

    [SerializeField]
    public List<GameObject> realClues;

    [Header("Found Information")]
    public List<GameObject> foundClues;

    public List<GameObject> foundAccusations;

    [Header("UI")]

    public TextMeshProUGUI timerText;

  
    void Start()
    {
        OnGameStateChanged(startState);
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

    public void NextState()
    {
        if (currentState == startState)
        {
            OnGameStateChanged(searchState);
        }
        else if (currentState == searchState)
        {
            OnGameStateChanged(players.Count > 2 ? voteState : endState);
        }
        else if (currentState == voteState)
        {
            OnGameStateChanged(players.Count > 2 ? endState : searchState);
        }
    }

    public void GenerateClueLists()
    {
        if (clues != null)
        {
            fakeClues.Clear();
            realClues.Clear();
            fakeClues = new List<GameObject>(clues);
            var shuffled = fakeClues.OrderBy(x => Random.value).ToList();
            realClues = shuffled.GetRange(0, realCluesNecessary);
            shuffled.RemoveRange(0, realClues.Count);
        }
    }


    public void FindPlayers()
    {
        players.Clear();
        var agents = GameObject.FindGameObjectsWithTag("Agent");
        players.AddRange(agents);
        var player = GameObject.FindGameObjectsWithTag("Player");
        players.AddRange(player);
    }


    public void DetermineEndCondition()
    {
        // win condition is met if most clues are found or rightfully accused is eliminated

    }
}

public interface IGameState
{
    public void OnEnter(GameStateManager gameStateManager);
    public void OnUpdate(GameStateManager gameStateManager);
    public void OnExit(GameStateManager gameStateManager);

}

