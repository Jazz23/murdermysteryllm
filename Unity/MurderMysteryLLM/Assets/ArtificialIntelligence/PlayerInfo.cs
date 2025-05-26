using System;

namespace ArtificialIntelligence.Agent;

/// <summary>
/// A player in the game. This can either be AI or human controlled.
/// </summary>
public class PlayerInfo
{
	public CharacterInformation CharacterInformation { get; init; }
	public string CurrentLocation { get; set; }
	public StoryContext StoryContext { get; init; }
}

public class Statement
{
	public IPlayer Speaker { get; init; }
	public string Text { get; init; }

	public override string ToString() => $"{Speaker.PlayerInfo.CharacterInformation.Name}: {Text}";
}

public class CharacterInformation
{
	public string Name { get; init; }
	public string Description { get; init; }
	public uint Age { get; init; }
	public string Occupation { get; init; }
	public string[] Traits { get; init; }
	public bool Murderer { get; init; }
}
