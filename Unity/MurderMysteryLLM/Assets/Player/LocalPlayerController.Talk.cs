using ArtificialIntelligence.StateMachine;
using UnityEngine;
using UnityEngine.InputSystem;

public partial class LocalPlayerController
{
    private TalkingAction _talkingAction;
    
    public async Task OnTalkedAt(IPlayer other, string message)
    {
        await Awaitable.MainThreadAsync();
        Chat.AddChatMessage($"{other.PlayerInfo.CharacterInformation.Name}: {message}");
        await Awaitable.BackgroundThreadAsync();
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
        InputSystem.actions.FindAction("Cancel").started -= OnCancel;
        Chat.ToggleChat();
        UnblockInput();
    }

    public void StopTalking()
    {
        _talkingAction = null;
    }
}