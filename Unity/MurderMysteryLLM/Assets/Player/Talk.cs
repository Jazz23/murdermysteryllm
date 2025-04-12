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
        LocalPlayerController.BlockInput();
        Chat.ToggleChat();
        InputSystem.actions.FindAction("Cancel").started += OnCancel;
    }

    private void OnCancel(InputAction.CallbackContext obj)
    {
        InputSystem.actions.FindAction("Cancel").started -= OnCancel;
        Chat.ToggleChat();
        LocalPlayerController.UnblockInput();
    }
}
