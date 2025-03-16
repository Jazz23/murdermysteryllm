using OpenAI.Chat;

namespace ArtificialIntelligence;

public class Storyteller
{
    public StoryContext StoryContext { get; }
}

public class Location
{
    public string Name { get; }
    public string[] ConnectingLocations { get; }

    public Location(string name, string[] connectingLocations)
    {
        Name = name;
        ConnectingLocations = connectingLocations;
    }
}

/// <summary>
/// Global context for the story that all players have access to the moment they enter the game. It is assumed
/// </summary>
public class StoryContext
{
    public Location[] LocationGraph { get; }
    public string[] CharacterNames { get; }
    
    public StoryContext(Location[] locationGraph, string[] characterNames)
    {
        LocationGraph = locationGraph;
        CharacterNames = characterNames;
    }
}