using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using ArtificialIntelligence;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// A singleton class attached to the textbox at the bottom of the screen. This handles text user input.
public class TextInput : MonoBehaviour
{
    private static TMP_InputField _textInput;
    
    // Use TaskCompletionSource to asynchronously wait until the user enters text
    private static TaskCompletionSource<string> userInputRequest;
    
    private void Start()
    {
        _textInput = GetComponent<TMP_InputField>();
    }

    public void OnTextInput()
    {
        var text = _textInput.text; // Just in case the user types something else super fast
        _textInput.text = "";
        
        userInputRequest?.SetResult(text);
    }
    
    /// <summary>
    /// Returns the next time the user enters text.
    /// </summary>
    /// <returns></returns>
    public static async Task<string> ReadTextFromUser()
    {
        Debug.Assert(userInputRequest == null, "Cannot request multiple text inputs at once!");
        
        // Post a request for text from the user
        userInputRequest = new TaskCompletionSource<string>();
        var text = await userInputRequest.Task;
        userInputRequest = null;
        return text;
    }

    /// <summary>
    /// Given a list of options, read text until the user types of them.
    /// </summary>
    public static async Task<string> PollUser(string[] validOptions)
    {
        while (true)
        {
            var text = await ReadTextFromUser();
            if (validOptions.Contains(text))
                return text;
            
            Debug.Log("Invalid input!");
            // TODO: Something more fancy here
        }
    }
}
