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
	[SerializeField] private List<GameObject> interactableItems = new List<GameObject>();
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

	public void FindIntractableGameObjects()
	{
		interactableItems = new List<GameObject>();
		Transform InteractableGameObject = GameObject.Find("Interactable").transform;
		if (InteractableGameObject == null) return;
		foreach (Transform child in InteractableGameObject)
			this.interactableItems.Add(child.gameObject);
	}

	public void GenerateClueLists()
	{
		if (interactableItems == null || interactableItems.Count == 0) return;

		fakeClues.Clear();
		realClues.Clear();

		fakeClues = new List<GameObject>(interactableItems);
		var shuffled = fakeClues.OrderBy(_ => Random.value).ToList();

		realClues = shuffled.Take(realCluesNecessary).ToList();
		fakeClues = shuffled.Skip(realCluesNecessary).ToList();
	}

	public void RetrievePlayerAgents(List<GameObject> agents = null)
	{
		players.Clear();
		players.AddRange(agents);
		players.AddRange(GameObject.FindGameObjectsWithTag("Player"));
		DetermineKiller();
	}

	private void DetermineKiller(int numberOfKillers = 0)
	{
		if (players.Count < MINIMUM_NUM_PLAYER_ALLOWED)
		{
			Debug.LogWarning("Not enough players to determine a killer.");
			return;
		}
		int randomIndex = Random.Range(0, players.Count);
		this.killer = players[randomIndex];
	}

	public async Task PlayTransition(string transitionText = "")
	{
		if (transitionAnimator == null)
		{
			Debug.LogWarning("Transition Animator is not set.");
			return;
		}

		UpdateTransitionUI(transitionText, isTransitioning: true);



		await Task.Delay((int)(transitionTime * 1000)); // Wait for the transition time

		UpdateTransitionUI(transitionText, isTransitioning: false);
	}

	bool IsAnimatorPlaying(Animator animator)
	{
		AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
		return stateInfo.normalizedTime < 1f || animator.IsInTransition(0);
	}


	private void UpdateTransitionUI(string transitionText, bool isTransitioning)
	{
		transitionStateDisplay.text = transitionText;
		countdownTimerDisplay.gameObject.SetActive(!isTransitioning);
		storyTellerUI.SetActive(!isTransitioning);
		transitionAnimator.SetBool("IsTransitioning", isTransitioning);
		foreach (var player in players)
		{
			player.gameObject.SetActive(!isTransitioning);
		}
	}
}


public interface IGameState
{
	Task OnEnter(GameStateManager gameStateManager);
	Task OnUpdate(GameStateManager gameStateManager);
	Task OnExit(GameStateManager gameStateManager);
}
