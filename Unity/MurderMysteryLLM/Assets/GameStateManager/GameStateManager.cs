using TMPro;
using UnityEditor;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UI;


public class GameStateManager : MonoBehaviour
{
    private const int minPlayers = 2;
    [Header("GameStates")]
    public IGameState currentState;
    [Tooltip("Time in seconds to search for clues")]
    [Range(0, 180)]
    public int searchTime = 75;

    [Tooltip("Time in seconds to vote")]
    [Range(0, 180)]
    public int voteTime = 75;
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

    public TextMeshProUGUI countdownTimerDisplay;

    public TextMeshProUGUI countdownTimerDisplayVote;
    public TextMeshProUGUI currentStateDisplay;

    public Animator transitionAnimator;

    void Start()
    {
        currentState = startState;
        currentState.OnEnter(this);
        // countdownTimerDisplay.gameObject.SetActive(false);
        // countdownTimerDisplay.text = "";
        // players = new List<GameObject>();
        // foundClues = new List<GameObject>();
        // foundAccusations = new List<GameObject>();
        // fakeClues = new List<GameObject>();
        // realClues = new List<GameObject>();
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

    public void ChangetoNextState()
    {
        currentState.OnExit(this);
        if (currentState == startState)
        {
            OnGameStateChanged(searchState);
        }
        else if (currentState == searchState)
        {
            OnGameStateChanged(players.Count > minPlayers ? voteState : endState);
        }
        else if (currentState == voteState)
        {
            OnGameStateChanged(players.Count > minPlayers ? endState : searchState);
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
            fakeClues = shuffled;
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


}

public interface IGameState
{
    public void OnEnter(GameStateManager gameStateManager);
    public void OnUpdate(GameStateManager gameStateManager);
    public void OnExit(GameStateManager gameStateManager);

}

