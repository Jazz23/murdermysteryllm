namespace ArtificialIntelligence
{
    /// <summary>
    /// Caches the contents of the given file to be used implicitly as a string. Also stores a static list of prompts.
    /// </summary>
    public class Prompt
    {
        // Static prompts
        public static readonly Prompt Innocent = new Prompt("AgentPrompts/innocentPrompt.txt");
        public static readonly Prompt Murderer = new Prompt("AgentPrompts/murdererPrompt.txt");
        public static readonly Prompt AgentSetup = new Prompt("AgentPrompts/agentSetupPrompt.txt");
        public static readonly Prompt AgentTalk = new Prompt("AgentPrompts/agentTalkPrompt.txt");
    
        // Member variables to cache the file contents
        private readonly string _fileLocation;
        private string? _value;
    
        public Prompt(string fileLocation) => _fileLocation = fileLocation;
    
        public static implicit operator string(Prompt prompt) => 
            prompt._value ??= Helpers.ReadFileFromRoot(prompt._fileLocation).Result;

        public override string ToString() => this;
    }
}

