using System;
using System.Linq;
using dotenv.net;
using OpenAI.Chat;

namespace ArtificialIntelligence;

public class Class1
{
    public string Hecker() => "heck";

    public string Test()
    {
        // Place .env in root of unity project
        DotEnv.Load();

        var chatClient = new ChatClient("gpt-4o-mini", Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

        // var response = chatClient.CompleteChat("Outta pure luck");
        var testingStoryContext = Helpers.CreateStoryContextFromJsonFile("StorytellerPrompts/storyObject.eg.jsonc").Result;
        // var agent1 = Helpers.CreateAgentFromJsonFile("AgentPrompts/ExampleData/character.jsonc", chatClient, "Grand Library", testingStoryContext).Result;
        // return response.Value.Content.First().Text;
        return "hi";
    }
}