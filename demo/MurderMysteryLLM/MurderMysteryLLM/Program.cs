using System.Text.Json;
using dotenv.net;
using MurderMysteryLLM;
using OpenAI.Chat;

// Load openai api key from .env file at the root of the project
DotEnv.Load(options: new DotEnvOptions(probeForEnv: true, probeLevelsToSearch: 4));

// Path to the root of the project containing prompt data
const string RootPath = "../../../../../../";

var chatClient = new ChatClient("gpt-4o-mini", Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
var agent1 = new AIAgent(chatClient, "Brenda");
var agent2 = new AIAgent(chatClient, "Chungus");
var storyteller = new Storyteller(chatClient);

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