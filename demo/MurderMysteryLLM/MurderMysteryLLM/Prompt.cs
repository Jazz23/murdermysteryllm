namespace MurderMysteryLLM;

/// <summary>
/// Caches the contents of the given file to be used implicitly as a string. Also stores a static list of prompts.
/// </summary>
/// <param name="fileLocation"></param>
public class Prompt(string fileLocation)
{
    // Static prompts
    public static readonly Prompt Innocent = new("AgentPrompts/innocentPrompt.txt");
    public static readonly Prompt Murderer = new("AgentPrompts/murdererPrompt.txt");
    public static readonly Prompt AgentSetup = new("AgentPrompts/agentSetupPrompt.txt");
    public static readonly Prompt AgentTalk = new("AgentPrompts/agentTalkPrompt.txt");
    
    // Member variables to cache the file contents
    private readonly string _fileLocation = fileLocation;
    private string? _value;
    
    public static implicit operator string(Prompt prompt) => 
        prompt._value ??= Helpers.ReadFileFromRoot(prompt._fileLocation).Result;

    public override string ToString() => this;
}