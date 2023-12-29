using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour, Initializer
{
    [SerializeField] private TMP_Text _roomName;
    [SerializeField] private Transform _player;
    [SerializeField] private Transform _playerList;
    [SerializeField] private GameObject _joinAndCreateWindow;
    [SerializeField] private GameObject _CreateRoomWindow;
    [SerializeField] private Button _exitButton;
    [SerializeField] private Button _startButton;

    public Unity.Services.Lobbies.Models.Player[] pla = new Unity.Services.Lobbies.Models.Player[4];

    public void Init()
    {
        LobbyManager.Instance.OnJoinedLobby += UpdateLobby_Event;
        LobbyManager.Instance.OnJoinedLobbyUpdate += UpdateLobby_Event;
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

        if(!LobbyManager.Instance._localLobbyUser.isHost)
        {
            _startButton.gameObject.SetActive(false);
        }

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
        int index = 0;
        ClearLobby();

        foreach (Unity.Services.Lobbies.Models.Player player in lobby.Players)
        {
            Transform p = Instantiate(_player);
            p.GetChild(0).GetComponent<TMP_Text>().text = player.Data["playerData"].Value;
            p.SetParent(_playerList);
            p.gameObject.SetActive(true);

            pla[index] = player;
            index++;
        }

        Show();
        HideOtherWindows();
    }

    private void ClearLobby()
    {
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

    private void StartGame()
    {
        if((LobbyManager.Instance.GetJoinedLobby().Players).Count != 4)
        {
            return;
        }

        //서버 -> 클라한테 신 불러오기
        //클라(생성, 초기화 등등) -> 서버한테 준비완료 알리기
        //카운트 다운
    }
}