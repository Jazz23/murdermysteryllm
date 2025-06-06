﻿using System;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using ArtificialIntelligence.Agent;
using OpenAI.Chat;

namespace ArtificialIntelligence;

public static class Helpers
{
	public static async Task<PlayerInfo> GetPlayerInfoFromJsonFile(string fileName, string currentLocation, StoryContext storyContext)
	{
		var jsonData = await File.ReadAllTextAsync(fileName);
		var characterInformation = JsonSerializer.Deserialize<CharacterInformation>(jsonData, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
		Debug.Assert(characterInformation != null, "Failed to deserialize agent JSON.");

		return new PlayerInfo
		{
			StoryContext = storyContext,
			CharacterInformation = characterInformation,
			CurrentLocation = currentLocation
		};
	}

	public static async Task<StoryContext> CreateStoryContextFromJsonFile(string fileName)
	{
		var jsonData = await File.ReadAllTextAsync(fileName);
		var storyContext = JsonSerializer.Deserialize<StoryContext>(jsonData, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
		Debug.Assert(storyContext != null, "Failed to deserialize story context JSON.");

		return storyContext;
	}
}