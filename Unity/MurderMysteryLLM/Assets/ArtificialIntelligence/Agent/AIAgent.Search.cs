using OllamaSharp.Models.Chat;
using UnityEngine;

public partial class AIAgent
{
    public HashSet<string> CluesFound { get; } = new();
    
    private AgentObserver _agentObserver;

    public void Search(GameObject obj)
    {
		_ = CluesFound.Add(obj.GetComponent<Searchable>().Search());
    }
    
    [AppendContext]
    private List<Message> AppendCluesFound()
    {
        return CluesFound.Select(x => new Message
        {
            Role = ChatRole.System,
            Content = x
        }).ToList();
    }
    
    [AppendContext]
    private List<Message> AppendNearbyObjects()
    {
        return new List<Message>()
        {
            new Message
            {
                Role = ChatRole.System,
                Content = _agentObserver.ConvertToContext()
            }
        };
    }
}