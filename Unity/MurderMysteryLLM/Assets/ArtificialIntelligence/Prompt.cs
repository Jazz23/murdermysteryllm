using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ArtificialIntelligence;

/// <summary>
/// Caches the contents of the given file to be used implicitly as a string. Also stores a static list of prompts.
///
/// Full explanation of this class and how it works here: https://www.youtube.com/watch?v=eLa24vb-W68
/// </summary>
public class Prompt
{
    // Singleton instances of the Prompt class
    public static readonly Prompt Innocent = new("AgentPrompts/innocentPrompt.txt");
    public static readonly Prompt Murderer = new("AgentPrompts/murdererPrompt.txt");
    public static readonly Prompt AgentSetup = new("AgentPrompts/agentSetupPrompt.txt");
    public static readonly Prompt AgentTalk = new("AgentPrompts/agentTalkPrompt.txt");
    public static readonly Prompt SummarizeConversation = new("AgentPrompts/summarizeConversation.txt");
    public static readonly Prompt TurnStart = new("AgentPrompts/turnStart.txt");
    public static readonly Prompt LocationQuery = new("AgentPrompts/locationQuery.txt");
    public static readonly Prompt PickWhoToTalkTo = new("AgentPrompts/pickWhoToTalkTo.txt");
    
    // Member variables to cache the file contents
    private readonly string _fileLocation;
    private string? _value;
    
    public Prompt(string fileLocation) => _fileLocation = fileLocation;
        
    public override string ToString() => this;
    
    public static implicit operator string(Prompt prompt)
    {
        Debug.Assert(prompt._value != null, "Please invoke Prompt.LoadPrompts before using the AI!");
        return prompt._value;
    }
    
    public static async Task LoadPrompts(string promptRootPath)
    {
        // Video explanation of this function: https://www.youtube.com/watch?v=eLa24vb-W68
        var staticPromptsLoadTasks = typeof(Prompt)
            .GetFields()
            .Where(x => x.FieldType == typeof(Prompt))
            .Select(async x =>
            {
                var instance = (Prompt)x.GetValue(null)!;
                var fullPath = Path.Combine(promptRootPath, instance._fileLocation);
                instance._value = await File.ReadAllTextAsync(fullPath);
            });

        await Task.WhenAll(staticPromptsLoadTasks);
    }
}