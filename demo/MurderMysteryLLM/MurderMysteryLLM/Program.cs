using dotenv.net;
using MurderMysteryLLM;

// Load openai api key from .env file at the root of the project
DotEnv.Load(options: new DotEnvOptions(probeForEnv: true, probeLevelsToSearch: 4));

var agent = new AIAgent();
Console.WriteLine(await agent.Test());