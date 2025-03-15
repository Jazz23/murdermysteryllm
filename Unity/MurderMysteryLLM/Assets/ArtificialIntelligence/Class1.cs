using System;
using System.Linq;
using System.Threading.Tasks;
using dotenv.net;
using OpenAI.Chat;

namespace ArtificialIntelligence
{
    public class Class1
    {
        public string Idgaf() => "hi there";
        public string Test()
        {
            DotEnv.Load(new DotEnvOptions(envFilePaths: new[] {"../../.env"}));

            var chatClient = new ChatClient("gpt-4o-mini", Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

            var response = chatClient.CompleteChat("Outta pure luck");
            return response.Value.Content.First().Text;
        }
    }
}