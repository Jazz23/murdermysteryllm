using ArtificialIntelligence;
using ArtificialIntelligence.StateMachine;

public class Searchable : Interactable
{
	public string clue = "This is an example clue.";

	public bool isRelatedToTheCrime = false;


	private void Awake()
	{

		if (this.hoverMessage != null)
		{
			this.hoverMessage = $"Search {this.gameObject.name}";
		}


	}
	public override void OnInteraction()
	{
		var player = LocalPlayerController.LocalPlayer;
		AIInterface.TurnStateMachine.QueueAction(new SearchAction { Item = gameObject, Player = player });
	}

	public string Search()
	{
		return clue;
	}
}