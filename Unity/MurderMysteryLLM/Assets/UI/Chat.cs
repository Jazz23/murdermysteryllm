using OpenAI.Chat;
using TMPro;
using UnityEngine;

public class Chat : MonoBehaviour
{
	private static Chat _instance;
	[SerializeField] private GameObject textPrefab;
	[SerializeField] private Transform textPanel;
	[SerializeField] private TMP_InputField inputBox;

	/// <summary>
	/// Who we are currently talking to.
	/// </summary>
	private IPlayer _other;


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
	public static void ToggleChat(IPlayer other = null)
	{
		if (other != null)
			_instance._other = other;

		var isActive = _instance.transform.GetChild(0).gameObject.activeSelf;

		foreach (Transform child in _instance.transform)
		{
			child.gameObject.SetActive(!isActive);
		}

		// Make sure the input box is ready to go when the UI opens
		if (!isActive)
			_instance.inputBox.ActivateInputField();
		else
		{
			foreach (Transform chatMessage in _instance.textPanel)
			{
				Destroy(chatMessage.gameObject);
			}
		}
	}

	public void OnInputBox()
	{
		if (inputBox.text == "" || !inputBox.gameObject.activeSelf)
			return;

		AddChatMessage($"{LocalPlayerController.LocalPlayer.PlayerInfo.CharacterInformation.Name}: {inputBox.text}");
		_ = _other.OnTalkedAt(LocalPlayerController.LocalPlayer, inputBox.text);
		inputBox.text = "";
		inputBox.ActivateInputField();
	}

	public static void AddChatMessage(string text)
	{
		TextMeshProUGUI textBox = Instantiate(_instance.textPrefab, _instance.textPanel)
			.GetComponent<TextMeshProUGUI>();

		textBox.fontSize = 30f;
		// textBox.font = Resources.Load<TMP_FontAsset>("XTypewriter-Regular");
		textBox.text = text;
	}
}