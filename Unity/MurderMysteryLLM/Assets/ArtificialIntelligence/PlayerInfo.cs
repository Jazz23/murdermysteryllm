using System;

namespace ArtificialIntelligence.Agent;

/// <summary>
/// A player in the game. This can either be AI or human controlled.
/// </summary>
public class PlayerInfo
{
    public CharacterInformation CharacterInformation { get; protected set; }
    public string CurrentLocation { get; set; }
    public readonly StoryContext StoryContext;

    public PlayerInfo(StoryContext storyContext, CharacterInformation characterInformation, string currentLocation)
    {
        CharacterInformation = characterInformation;
        CurrentLocation = currentLocation;
        StoryContext = storyContext;
    }
}

public class Statement
{
    public string Speaker { get; }
    public string Text { get; }

    public Statement(string speaker, string text)
    {
        Speaker = speaker;
        Text = text;
    }

    public override string ToString() => $"{Speaker}: {Text}";
}

public class CharacterInformation
{
    public string Name { get; }
    public string Description { get; }
    public uint Age { get; }
    public string Occupation { get; }
    public string[] Traits { get; }
    public bool Murderer { get; }

    public CharacterInformation(string name, string description, uint age, string occupation, string[] traits,
        bool murderer)
    {
        Name = name;
        Description = description;
        Age = age;
        Occupation = occupation;
        Traits = traits;
        Murderer = murderer;
    }
}