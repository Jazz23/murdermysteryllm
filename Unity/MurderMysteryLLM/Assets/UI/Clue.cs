using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Clue : MonoBehaviour
{
    private static Clue _instance;
    [SerializeField] private GameObject cluePanel;
    [SerializeField] private float clueDisplaySeconds = 5;

    [SerializeField] private GameObject stickyPanel;
    [SerializeField] private GameObject stickyNotePrefab;
    [SerializeField] private float stickySpacing;
    [SerializeField] private Transform stickyStartPos;

    private readonly List<GameObject> _clueStickys = new();

    public void Awake()
    {
        _instance = this;
    }

    /// <summary>
    /// Shows a large note in the center of the screen for some time.
    /// </summary>
    /// <param name="clue"></param>
    public static void DisplayClue(string clue) => _instance.DisplayClueHelper(clue);

    private void DisplayClueHelper(string clue)
    {
        cluePanel.GetComponentInChildren<TextMeshProUGUI>().text = clue;
        cluePanel.SetActive(true);

        Task.Run(async () =>
        {
            float elapsedTime = 0f;
            while (elapsedTime < clueDisplaySeconds)
            {
            await Awaitable.MainThreadAsync();

            if (Input.GetButtonDown("Cancel"))
            {
                cluePanel.SetActive(false);
                return;
            }

            await Awaitable.WaitForSecondsAsync(0.1f);
            elapsedTime += 0.1f;
            }

            cluePanel.SetActive(false);
        });
    }

    /// <summary>
    /// Given a hashset of strings, generates visible sticky notes at the top of the screen.
    /// </summary>
    public static void UpdateClueStickys(HashSet<string> cluesFound) =>
        _instance.UpdateClueStickysHelper(cluesFound);

    private void UpdateClueStickysHelper(HashSet<string> cluesFound)
    {
        // Reset old sticky notes
        _clueStickys.ForEach(Destroy);
        _clueStickys.Clear();

        // Instantiate new ones
        for (var i = 0; i < cluesFound.Count; i++)
        {
            var pos = stickyStartPos.position;
            pos.x -= i * stickySpacing;
            var newSticky = Instantiate(stickyNotePrefab, stickyPanel.transform);
            newSticky.transform.position = pos;
            newSticky.GetComponent<Sticky>().clueText = cluesFound.ElementAt(i);

            _clueStickys.Add(newSticky);
        }
    }
}