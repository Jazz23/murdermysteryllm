using OpenAI.Chat;

namespace MurderMysteryLLM;

public class AIAgent
{
    private readonly ChatClient _chatClient = new("gpt-4o-mini", Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
    
    public async Task<string> Test() => (await _chatClient.CompleteChatAsync("What is your name?")).Value.Content.First().Text;
}