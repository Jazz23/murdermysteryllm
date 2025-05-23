﻿using System.Text.Json;using ArtificialIntelligence;
using ArtificialIntelligence.Agent;
using dotenv.net;
using OpenAI.Chat;

// Place .env file at the root of the project
DotEnv.Load(options: new DotEnvOptions(probeForEnv: true, probeLevelsToSearch: 5));

// Path to the root of the project containing prompt data

// Console.WriteLine(new Class1().Test());
//
// return;

var chatClient = new ChatClient("gpt-4o-mini", Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
var promptPathPrefix = Environment.GetEnvironmentVariable("PROMPTS_PATH")!;
await Prompt.LoadPrompts(promptPathPrefix);

var testingStoryContext = await Helpers.CreateStoryContextFromJsonFile("StorytellerPrompts/storyObject.eg.jsonc");
var agent1 = await Helpers.CreateAgentFromJsonFile("AgentPrompts/ExampleData/character.jsonc", chatClient, "Grand Library", testingStoryContext);
var agent2 = await Helpers.CreateAgentFromJsonFile("AgentPrompts/ExampleData/character2.jsonc", chatClient, "Grand Library", testingStoryContext);

var testStatement = new Statement(agent1.CharacterInformation.Name, $"Hello {agent2.CharacterInformation.Name}. Did you also see the bloody knife?");
agent1.InjectTestConversation([testStatement]);
agent2.InjectTestConversation([testStatement]);

var counter = 0;
while (true)
{
    counter++;
    if (counter % 2 == 0)
        Console.WriteLine(await agent1.SpeakTo(agent2));
    else
        Console.WriteLine(await agent2.SpeakTo(agent1));

    Console.ReadLine();
}