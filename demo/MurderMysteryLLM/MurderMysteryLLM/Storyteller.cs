using OpenAI.Chat;

namespace MurderMysteryLLM;

// public class Storyteller(ChatClient chatClient)
// {
//     public StoryContext StoryContext { get; }
// }

public record Location(string Name, string[] ConnectingLocations);

public record StoryContext(Location[] LocationGraph, string[] CharacterNames);