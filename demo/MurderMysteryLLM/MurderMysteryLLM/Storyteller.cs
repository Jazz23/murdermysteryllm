using OpenAI.Chat;

namespace MurderMysteryLLM;

public class Storyteller(ChatClient chatClient)
{
    public required Location[] LocationGraph { get; init; }
}

public record Location(string name, Location[] connectingLocations);