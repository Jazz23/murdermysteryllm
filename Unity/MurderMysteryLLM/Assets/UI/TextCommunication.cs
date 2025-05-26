using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using ArtificialIntelligence;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// A singleton class attached to the textbox at the bottom of the screen. This handles text user input and sends
// the data to the server.
// Video documentation: https://gist.github.com/Jazz23/1d9b8b2468f504c80f874ff3155e9569
[RequireComponent(typeof(TMP_InputField))]
public class TextCommunication : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI _storytellerText;

	private static TMP_InputField _textInput;
	private static TextCommunication _instance;

	private string[] _validOptions;
	// Wait until the server receives the text from the RPC
	private static AwaitableCompletionSource<string> serverInputRequest;

	private void Awake()
	{
		_instance = this;
	}

	// No ownership stuff since this is a scene object
	private void ReplyWithText(string text)
	{
		serverInputRequest.SetResult(text);
	}

	/// <summary>
	/// Send an RPC to the owner to display text
	/// </summary>
	public static void DisplayStorytellerText(string message)
	{
		_instance._storytellerText.text = message;
	}
}
