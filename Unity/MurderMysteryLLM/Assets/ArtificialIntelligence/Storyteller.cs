namespace ArtificialIntelligence
{
    // public class Storyteller(ChatClient chatClient)
// {
//     public StoryContext StoryContext { get; }
// }

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
}