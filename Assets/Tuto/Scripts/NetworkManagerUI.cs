using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport;
using Unity.Networking.Transport.Relay;


public class NetworkManagerUI : MonoBehaviour
{

    [SerializeField] private Button createButton;
    [SerializeField] private Button joinButton;
    [SerializeField] private GameObject joinInput;
    //[SerializeField] private Button startButton;
    [SerializeField] private Canvas _mainMenu;
    [SerializeField] private Canvas _aimUI;

    [SerializeField] private GameObject joinCodeText;

    private void Awake(){

        createButton.onClick.AddListener(() => {
            CreateRelay();
        });

        joinButton.onClick.AddListener(() => {
            JoinRelay(joinInput.GetComponent<TMP_InputField>().text);
        });

        // startButton.onClick.AddListener(() => {
        //     startGame();
        // });
        _aimUI.enabled = false;

    }

    private async void Start(){
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () => {
            Debug.Log("Signed in " + AuthenticationService.Instance.PlayerId);
        };

        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    private async void CreateRelay(){
        try {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(3);

            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            joinCodeText.GetComponent<TMPro.TextMeshProUGUI>().text = joinCode;
            
            Debug.Log("code : " + joinCode);

            RelayServerData relayServerData = new RelayServerData(allocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartHost();
            _mainMenu.enabled = false;
            _aimUI.enabled = true;
        } catch (RelayServiceException e){
            Debug.Log("Relay service error: " + e.Message);
        }
    }

    private async void JoinRelay(string joinCode){
        try {
            Debug.Log("JoinRelay: " + joinCode);
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            RelayServerData relayServerData = new RelayServerData(joinAllocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartClient();
            _mainMenu.enabled = false;
            _aimUI.enabled = true;
        } catch (RelayServiceException e){
            Debug.Log("Relay service error: " + e.Message);
        }
    }

    private void startGame(){
        
    }

}