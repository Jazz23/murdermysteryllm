using UnityEditor;
using UnityEditor.Search;
using UnityEngine;


public class GameStateManager : MonoBehaviour
{

    [Header("GameStates")]
    private IGameState currentState;

    private StartState startState = new StartState();
    private SearchState searchState = new SearchState();
    private VoteState voteState = new VoteState();
    private EndState endState = new EndState();

    [Header("Scene")]

    [SerializeField]
    public List<GameObject> players;

    [SerializeField]
    private GameObject killer;


    [Header("Clues")]

    [Range(0, 14)]
    [SerializeField]
    private int realCluesNecessary = 10;

    [SerializeField]
    private List<GameObject> clues;

    [SerializeField]
    private List<GameObject> fakeClues;

    [SerializeField]
    private List<GameObject> realClues;



    private void Awake()
    {
        // Create Scenario (who died)

        // Set killer

        // Generate Clues in scene

        // Create win condition ( whos is killer and clues)

        // set game State
        FindPlayers();

        int randomIndex = Random.Range(0, players.Count);
        killer = players[randomIndex];

        if (clues != null)
        {
            fakeClues.Clear();
            realClues.Clear();


            fakeClues = new List<GameObject>(clues);

            var shuffled = fakeClues.OrderBy(x => Random.value).ToList();
            realClues = shuffled.GetRange(0, realCluesNecessary);
            shuffled.RemoveRange(0, realClues.Count);
        }

        OnGameStateChanged(startState);


    }



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

    private void FindPlayers()
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

