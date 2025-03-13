using System.Diagnostics;
using System.Text.Json;
using OpenAI.Chat;

namespace MurderMysteryLLM;

public static class Helpers
{
    private const string RootPath = "../../../../../../";
    
    public static Task<string> ReadFileFromRoot(string fileName) => File.ReadAllTextAsync(RootPath + fileName);
    
    public static async Task<AIAgent> CreateAgentFromJsonFile(string fileName, ChatClient chatClient, string currentLocation)
    {
        var jsonData = await ReadFileFromRoot(fileName);
        var json = JsonSerializer.Deserialize<CharacterInformation>(jsonData, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase});
        Debug.Assert(json != null, "Failed to deserialize agent JSON.");
        return new AIAgent(chatClient, json, currentLocation);
    }
}