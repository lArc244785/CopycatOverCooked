using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using CopycatOverCooked.GamePlay;

public class LobbyUI : NetworkBehaviour, Initializer
{
    [SerializeField] private TMP_Text _roomName;
    [SerializeField] private Transform _player;
    [SerializeField] private Transform _playerList;
    [SerializeField] private GameObject _joinAndCreateWindow;
    [SerializeField] private GameObject _CreateRoomWindow;
    [SerializeField] private Button _exitButton;
    [SerializeField] private Button _startButton;
    [SerializeField] private GameObject _ordermanager;
    [SerializeField] private Image _background;

    private LobbyListUI _lobbyListUI;
    //public Unity.Services.Lobbies.Models.Player[] pla = new Unity.Services.Lobbies.Models.Player[4];

    public void Init()
    {
        LobbyManager.Instance.OnJoinedLobby += UpdateLobby_Event;
        LobbyManager.Instance.OnJoinedLobbyUpdate += UpdateLobby_Event;
        LobbyManager.Instance.OnLeftLobby += LobbyManager_OnLeftLobby;
    }

    public void Update()
    {
        LobbyManager.Instance.HandleLobbyHeartbeat();
        LobbyManager.Instance.HandleLobbyPolling();
    }

    private void Start()
    {
        _exitButton.onClick.AddListener(ShowOtherWindows);
        _startButton.onClick.AddListener(StartGame);

        if (!LobbyManager.Instance._localLobbyUser.isHost)
        {
            _startButton.gameObject.SetActive(false);
        }

        _lobbyListUI = new LobbyListUI();
        //LobbyManager.Instance._users.onNumberOfPeopleChanged += UpdateLobby;
    }

    private void UpdateLobby_Event(object sender, LobbyManager.LobbyEventArgs e)
    {
        UpdateLobby();
    }

    private void UpdateLobby()
    {
        UpdateLobby(LobbyManager.Instance.GetJoinedLobby());
    }

    private void UpdateLobby(Lobby lobby)
    {
        _roomName.text = lobby.Name;

        ClearLobby();

        foreach (Unity.Services.Lobbies.Models.Player player in lobby.Players)
        {
            Transform p = Instantiate(_player);
            p.GetChild(0).GetComponent<TMP_Text>().text = player.Data["playerData"].Value;
            p.SetParent(_playerList);
            p.gameObject.SetActive(true);
        }

        GameManager.instance.loadPlayCount = lobby.Players.Count;

        Show();
        HideOtherWindows();
    }

    private void ClearLobby()
    {
        if (GameManager.instance.state.Value != GameFlow.Lobby)
            return;

        foreach (Transform child in _playerList)
        {
            if (child == _player)
            {
                continue;
            }

            Destroy(child.gameObject);
        }
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void HideOtherWindows()
    {
        _joinAndCreateWindow.SetActive(false);
        _CreateRoomWindow.SetActive(false);
    }

    private void ShowOtherWindows()
    {
        _joinAndCreateWindow.SetActive(true);
        _CreateRoomWindow.SetActive(true);

        Hide();
    }

    private void LobbyManager_OnLeftLobby(object sender, System.EventArgs e)
    {
        ClearLobby();
        Hide();
    }

    private void StartGame()
    {
        /*if ((LobbyManager.Instance.GetJoinedLobby().Players).Count != 4)
        {
            return;
        }*/

        //showServerRpc();
        gameObject.SetActive(false);
        //gameObject.GetComponent<NetworkObject>().Despawn();
        GameManager.instance.StartGameServerRpc();
    }

    [ServerRpc]
    private void showServerRpc()
    {
        Debug.Log("서버rpc");
        showSceneClientRpc();
    }

    [ClientRpc]
    private void showSceneClientRpc()   
    {
        Debug.Log("클라rpc");
        _background.gameObject.SetActive(false);
        this.gameObject.SetActive(false);

        _ordermanager.gameObject.SetActive(true);
    }
}