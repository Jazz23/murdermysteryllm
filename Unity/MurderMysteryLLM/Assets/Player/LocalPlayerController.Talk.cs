using ArtificialIntelligence.StateMachine;
using UnityEngine.InputSystem;

public partial class LocalPlayerController
{
    private TalkingAction _talkingAction;
    
    public void OnTalkedAt(IPlayer other, string message)
    {
        Chat.AddChatMessage($"{other.PlayerInfo.CharacterInformation.Name}: {message}");
    }
    
    public void StartTalking(TalkingAction action)
    {
        _talkingAction = action;
        BlockInput();
        Chat.ToggleChat(action.Other);
        InputSystem.actions.FindAction("Cancel").started += OnCancel;
    }

    private void OnCancel(InputAction.CallbackContext obj)
    {
        _talkingAction.EndConversation();
        _talkingAction = null;
        InputSystem.actions.FindAction("Cancel").started -= OnCancel;
        Chat.ToggleChat();
        UnblockInput();
    }
}