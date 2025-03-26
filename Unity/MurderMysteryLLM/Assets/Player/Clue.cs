using TMPro;
using UnityEngine;

public class Clue : MonoBehaviour
{
    private static Clue _instance;
    [SerializeField] private GameObject stickyNotePrefab;

    public void Awake()
    {
        _instance = this;
        gameObject.SetActive(false);
    }

    public static void DisplayClue(string clue) => _instance.DisplayClueHelper(clue);

    private void DisplayClueHelper(string clue)
    {
        GetComponentInChildren<TextMeshProUGUI>().text = clue;
        gameObject.SetActive(true);
        Task.Run(async () =>
        {
            await Awaitable.MainThreadAsync();
            await Awaitable.WaitForSecondsAsync(5);
            gameObject.SetActive(false);
        });

    }
}