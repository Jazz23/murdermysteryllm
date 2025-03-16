using System;
using System.Collections;
using System.IO;
using ArtificialIntelligence;
using dotenv.net;
using FishNet.Object;
using OpenAI.Chat;
using UnityEngine;

/// <summary>
/// Used to interface Unity with AI library.
/// </summary>
public class AIInterface : NetworkBehaviour
{
    public static StoryContext StoryContext;
    public static ChatClient ChatClient;

    public bool MockStoryContext = true;
    
    public override void OnStartServer()
    {
        // Only the server should mess around with the AI library
        StartCoroutine(InitAILibrary());
    }
    
    // Loads prompts and any testing data
    private IEnumerator InitAILibrary()
    {
        // Place .env in root of unity project
        DotEnv.Load();
        
        ChatClient = new ChatClient("gpt-4o-mini", Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
        
        // Load prompts from local text files
        var promptPathPrefix = Environment.GetEnvironmentVariable("PROMPTS_PATH")!;
        yield return Prompt.LoadPrompts(promptPathPrefix).WaitUntil();

        if (MockStoryContext)
        {
            // If we're mocking story context, load from a file instead of generating it
            var loadStoryContextTask = Helpers.CreateStoryContextFromJsonFile(
                Path.Combine(promptPathPrefix, "StorytellerPrompts/storyObject.eg.jsonc"));
            yield return loadStoryContextTask.WaitUntil();
            StoryContext = loadStoryContextTask.Result; // Temporary: This will be generated via ChatGPT in the future
        }
    }
}