using ArtificialIntelligence.StateMachine;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// This class is attached to any game object that can be talked to, IE agents.
/// </summary>
public class Talk : Interactable
{
	public string hoverMessageTemplate = "Talk to {0}?";

	public override void OnHoverStay()
	{
		// If they just spawned in, they don't have a name yet
		if (GetComponent<IPlayer>().PlayerInfo?.CharacterInformation?.Name is not { } playerName)
			return;
		hoverMessage = string.Format(hoverMessageTemplate, playerName);
	}

	public override void OnInteraction()
	{
		AIInterface.TurnStateMachine.QueueAction(new TalkingAction
		{
			Player = LocalPlayerController.LocalPlayer, // Only the local player can interact
			Other = GetComponent<IPlayer>(), // We are being interacted with
		});
	}
}
