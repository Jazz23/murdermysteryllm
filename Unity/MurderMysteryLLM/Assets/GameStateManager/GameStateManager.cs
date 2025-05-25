using TMPro;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class GameStateManager : MonoBehaviour
{
    private static GameStateManager _instance;

    private const int MINIMUM_NUM_PLAYER_ALLOWED = 2;

    [Header("Game States")]
    public IGameState currentState;
    [Tooltip("Time in seconds to search for clues")]
    [Range(0, 180)] public int searchTime = 75;
    [Tooltip("Time in seconds to vote")]
    [Range(0, 180)] public int voteTime = 75;

    public StartState startState = new StartState();
    public SearchState searchState = new SearchState();
    public VoteState voteState = new VoteState();
    public EndState endState = new EndState();

    [Header("Scene")]
    public List<GameObject> players = new List<GameObject>();
    public GameObject killer;

    [Header("Clues")]
    [Range(0, 14)][SerializeField] private int realCluesNecessary = 10;
    [SerializeField] private List<GameObject> clues = new List<GameObject>();
    [SerializeField] private List<GameObject> fakeClues = new List<GameObject>();
    [SerializeField] private List<GameObject> realClues = new List<GameObject>();

    [Header("Found Information")]
    public List<GameObject> foundClues = new List<GameObject>();
    public List<GameObject> foundAccusations = new List<GameObject>();

    [Header("UI")]

    public GameObject storyTellerUI;
    public TextMeshProUGUI countdownTimerDisplay;
    public TextMeshProUGUI transitionStateDisplay;
    public Animator transitionAnimator;

    public float transitionTime = 5f;

    private bool pressedOnce = false;

    public static GameStateManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindAnyObjectByType<GameStateManager>();
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject(nameof(GameStateManager));
                    _instance = singletonObject.AddComponent<GameStateManager>();
                }
            }
            return _instance;
        }
    }

    private async void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

        await Awaitable.MainThreadAsync();
    }

    private async void Start()
    {
        currentState = startState;
        await currentState.OnEnter(this);
    }

    private async void Update()
    {
        await currentState.OnUpdate(this);
    }

    public async Task OnGameStateChanged(IGameState newState)
    {
        if (currentState != null)
        {
            await currentState.OnExit(this);
        }
        currentState = newState;
        await currentState.OnEnter(this);
    }

    public async Task ChangeToNextState()
    {
        if (currentState == startState)
        {
            await OnGameStateChanged(searchState);
        }
        else if (currentState == searchState)
        {
            await OnGameStateChanged(voteState);
        }
        else if (currentState == voteState)
        {
            await OnGameStateChanged(endState);
        }
    }

    public void FindClues()
    {
        clues = new List<GameObject>();
        Transform InteractableGameObject = GameObject.Find("Interactable").transform;
        if (InteractableGameObject == null) return;
        foreach (Transform child in InteractableGameObject)
            this.clues.Add(child.gameObject);
    }

    public void GenerateClueLists()
    {
        if (clues == null || clues.Count == 0) return;

        fakeClues.Clear();
        realClues.Clear();

        fakeClues = new List<GameObject>(clues);
        var shuffled = fakeClues.OrderBy(_ => Random.value).ToList();

        realClues = shuffled.Take(realCluesNecessary).ToList();
        fakeClues = shuffled.Skip(realCluesNecessary).ToList();
    }

    public void FindPlayers()
    {
        players.Clear();
        players.AddRange(GameObject.FindGameObjectsWithTag("Agent"));
        players.AddRange(GameObject.FindGameObjectsWithTag("Player"));
    }

    public async Task PlayTransition(string transitionText = "")
    {
        Debug.Log("PlayTransition");
        this.transitionStateDisplay.text = transitionText;
        if (transitionAnimator == null)
        {
            Debug.LogWarning("Transition Animator is not set.");
            return;
        }
        countdownTimerDisplay.gameObject.SetActive(false);
        storyTellerUI.SetActive(false);
        transitionAnimator.SetBool("IsTransitioning", true);

        await Task.Delay((int)(transitionTime * 1000)); // Wait for the transition time
        transitionAnimator.SetBool("IsTransitioning", false);
        countdownTimerDisplay.gameObject.SetActive(true);
        storyTellerUI.SetActive(true);
    }
}

public interface IGameState
{
    Task OnEnter(GameStateManager gameStateManager);
    Task OnUpdate(GameStateManager gameStateManager);
    Task OnExit(GameStateManager gameStateManager);
}
