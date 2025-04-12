using OpenAI.Chat;
using TMPro;
using UnityEngine;

public class Chat : MonoBehaviour
{
    [SerializeField] private GameObject textPrefab;
    [SerializeField] private Transform textPanel;
    
    [SerializeField] private TMP_InputField inputBox;

    private static Chat _instance;

    private void Awake() => _instance = this;

    /// <summary>
    /// A list of text boxes that will get destroyed with each update.
    /// </summary>
    private readonly List<GameObject> _oldMessages = new();

    public static void UpdateChat(ChatMessage[] chatLog) =>
        _instance.UpdateChatHelper(chatLog);
    
    private void UpdateChatHelper(ChatMessage[] chatLog)
    {
        _oldMessages.ForEach(Destroy);
        _oldMessages.Clear();
        
        foreach (var message in chatLog)
        {
            var textbox = Instantiate(textPrefab, textPanel);
            textbox.GetComponent<TextMeshProUGUI>().text = message.Content.First().Text;
            _oldMessages.Add(textbox);
        }
    }

    /// <summary>
    /// Sets child objects of "ChatStuff" to be active or inactive.
    /// </summary>
    public static void ToggleChat()
    {
        var isActive = _instance.transform.GetChild(0).gameObject.activeSelf;
        
        foreach (Transform child in _instance.transform)
        {
            child.gameObject.SetActive(!isActive);
        }
    }

    public void OnInputBox()
    {
        //...
        

        inputBox.text = "";
    }
}