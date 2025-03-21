using FishNet;
using TMPro;
using UnityEngine;

public class ConnectToIP : MonoBehaviour
{
    [SerializeField] private TMP_InputField _ipAddressInput;
        
    public void OnConnectButtonPressed()
    {
        InstanceFinder.ClientManager.StartConnection(_ipAddressInput.text);
    }
}
