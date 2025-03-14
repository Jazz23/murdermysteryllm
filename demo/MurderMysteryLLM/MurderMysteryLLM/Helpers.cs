using System.Diagnostics;
using System.Text.Json;
using OpenAI.Chat;

namespace MurderMysteryLLM;

public static class Helpers
{
    private const string RootPath = "../../../../../../";
    
    public static string InnocentPrompt => InnocentPromptCache ??= ReadFileFromRoot("AgentPrompts/innocentPrompt.txt").Result;
    private static string? InnocentPromptCache;

    public static string MurdererPrompt =>
        MurdererPromptCache ??= ReadFileFromRoot("AgentPrompts/murdererPrompt.txt").Result;
    private static string? MurdererPromptCache;
    
    public static string AgentSetupPrompt => AgentSetupPromptCache ??= ReadFileFromRoot("AgentPrompts/agentSetupPrompt.txt").Result;
    private static string? AgentSetupPromptCache;
    
    public static Task<string> ReadFileFromRoot(string fileName) => File.ReadAllTextAsync(RootPath + fileName);
    
    public static async Task<AIAgent> CreateAgentFromJsonFile(string fileName, ChatClient chatClient, string currentLocation, StoryContext storyContext)
    {
        var jsonData = await ReadFileFromRoot(fileName);
        var characterInformation = JsonSerializer.Deserialize<CharacterInformation>(jsonData, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase});
        Debug.Assert(characterInformation != null, "Failed to deserialize agent JSON.");
        
        return new AIAgent(chatClient, characterInformation, currentLocation, storyContext);
    }
    
    public static async Task<StoryContext> CreateStoryContextFromJsonFile(string fileName)
    {
        var jsonData = await ReadFileFromRoot(fileName);
        var storyContext = JsonSerializer.Deserialize<StoryContext>(jsonData, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase});
        Debug.Assert(storyContext != null, "Failed to deserialize story context JSON.");
        
        return storyContext;
    }
}