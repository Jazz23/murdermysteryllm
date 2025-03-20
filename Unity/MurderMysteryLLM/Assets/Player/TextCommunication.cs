using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using ArtificialIntelligence;
using FishNet.Connection;
using FishNet.Object;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// A singleton class attached to the textbox at the bottom of the screen. This handles text user input and sends
// the data to the server
[RequireComponent(typeof(TMP_InputField))]
public class TextCommunication : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI _storytellerText;
    
    private static TMP_InputField _textInput;
    private static TextCommunication _instance;
    
    // Use TaskCompletionSource to asynchronously wait until the user enters text
    private static TaskCompletionSource<string> textboxInputRequest;
    private string[] _validOptions;
    // Wait until the server receives the text from the RPC
    private static TaskCompletionSource<string> serverInputRequest;
    
    private void Awake()
    {
        _textInput = GetComponent<TMP_InputField>();
        _instance = this;
    }

    public void OnTextInput()
    {
        // For some reason this triggers after tabbing in and out
        if (_textInput.text == "")
            return;
        // Cache the text since async causes some funky stuff with execution order
        var text = _textInput.text;
        _textInput.text = "";
        
        textboxInputRequest?.TrySetResult(text);
    }

    /// <summary>
    /// Send the user a message and waits for a valid response.
    /// </summary>
    [Server]
    public static async Task<string> PollUser(NetworkConnection conn, string[] validOptions, string message)
    {
        serverInputRequest = new TaskCompletionSource<string>();
        _instance.PostMessageAndRequestTextRpc(conn, validOptions, message);
        return await serverInputRequest.Task;
    }

    [TargetRpc]
    private void PostMessageAndRequestTextRpc(NetworkConnection conn, string[] validOptions, string message)
    {
        _storytellerText.text = message;
        Task.Run(() => RequestText(validOptions, message));
    }

    private async Awaitable RequestText(string[] validOptions, string message)
    {
        await Awaitable.MainThreadAsync(); // Since we're manipulating UI
        
        // Continuously poll the user until a valid input is typed into the textbox
        string text;
        do
        {
            // We get the text from the user by assigning a new textboxInputRequest, then awaiting for it's value.
            // The OnTextInput event will set the value (result) of this new TaskCompletionSource.
            text = await (textboxInputRequest = new TaskCompletionSource<string>()).Task;
            if (!validOptions.Contains(text))
                _storytellerText.text = $"Invalid input {text}. Must be one of {string.Join(", ", validOptions)}";
        } while (!validOptions.Contains(text));

        _storytellerText.text = "";
        ReplyWithText(text);
    }

    // No ownership stuff since this is a scene object
    [ServerRpc(RequireOwnership = false)]
    private void ReplyWithText(string text)
    {
        serverInputRequest.SetResult(text);
    }

    public static void DisplayStorytellerText(NetworkConnection conn, string message)
    {
        _instance.DisplayTextRpc(conn, message);
    }
    
    [TargetRpc]
    private void DisplayTextRpc(NetworkConnection conn, string message) => _storytellerText.text = message;
}
