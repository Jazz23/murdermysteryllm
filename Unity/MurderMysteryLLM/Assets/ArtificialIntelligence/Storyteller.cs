using System.Linq;
using System.Threading.Tasks;
using ArtificialIntelligence.Agent;
using OpenAI.Chat;

namespace ArtificialIntelligence;

public class Storyteller
{
	// TODO: Read prompts from file instead
}

public class Location
{
	public string Name { get; init; }
	public string[] ConnectingLocations { get; init; }
}

/// <summary>
/// Global context for the story that all players have access to the moment they enter the game. It is assumed
/// </summary>
public class StoryContext
{
	public Location[] LocationGraph { get; init; }
	public string[] CharacterNames { get; init; }
}