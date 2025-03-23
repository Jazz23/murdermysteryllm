using System.Linq;
using System.Threading.Tasks;
using ArtificialIntelligence.Agent;
using OpenAI.Chat;

namespace ArtificialIntelligence;

public class Storyteller
{
    // TODO: Read prompts from file instead
    
    /// <summary>
    /// Ask the player what their turn should be.
    /// </summary>
    public async Task PromptTurn(IPlayer player)
    {
        var chosenAction = await player.TakeTurn("Respond with either search, talk, door, or vote.");
        switch (chosenAction)
        {
            case PlayerActions.DOOR:
                await PromptDoor(player);
                break;
        }
    }

    /// <summary>
    /// Ask the player which door they want to enter.
    /// </summary>
    /// <param name="player"></param>
    public async Task PromptDoor(IPlayer player)
    {
        // Find adjacent locations to the player and format them into a comma separated string
        var adjacentLocations = player.PlayerInfo.StoryContext.LocationGraph
            .Where(x => x.ConnectingLocations.Contains(player.PlayerInfo.CurrentLocation))
            .Select(x => x.Name.ToLower())
            .ToArray();
        var formatted = string.Join(", ", adjacentLocations);
        
        var chosenDoor = await player.AskDoor(adjacentLocations, $"Which room would you like to enter? {formatted}?");
        player.TakeDoor(chosenDoor, $"You decide to enter the {chosenDoor}. You feel the breeze across your face as you walk.");
    }
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