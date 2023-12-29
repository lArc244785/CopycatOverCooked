using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyListUI : MonoBehaviour
{
    [SerializeField] private Transform _room;
    [SerializeField] private Button _join;
    [SerializeField] private Button _create;
    [SerializeField] private Button _search;
    [SerializeField] private Button _refresh;
    [SerializeField] private RectTransform _roomPosition;
    [SerializeField] private Transform _createRoomUI;

    private void Awake()
    {
        //_join
        _create.onClick.AddListener(CreateLobbyButtonClick);
        //_search
        _refresh.onClick.AddListener(RefreshLobbyListButtonClick);
        
    }

    void Start()
    {
        LobbyManager.Instance.OnLobbyListChanged += LobbyManager_OnLobbyListChanged;
        LobbyManager.Instance.OnJoinedLobby += LobbyManager_OnJoinedLobby;
        LobbyManager.Instance.onLobbyCreated += RefreshLobbyListButtonClick;
    }
    private void RefreshLobbyListButtonClick()
    {        
        LobbyManager.Instance.RefreshLobbyList();
    }

    private void CreateLobbyButtonClick()
    {
        _createRoomUI.gameObject.SetActive(true);
    }

    private void ClearLobbyList()
    {
        foreach (Transform child in _roomPosition)
        {
            if (child == _room)
            {
                continue;
            }

            Destroy(child.gameObject);
        }
    }

    private void LobbyManager_OnJoinedLobby(object sender, LobbyManager.LobbyEventArgs e)
    {
        //gameObject.SetActive(false);
    }
    private void LobbyManager_OnLobbyListChanged(object sender, LobbyManager.OnLobbyListChangedEventArgs e)
    {
        UpdateLobbyList(e.lobbyList);
    }

    private void UpdateLobbyList(List<Lobby> lobbyList)
    {
        foreach (Transform child in _roomPosition)
        {
            if (child == _room)
            {
                continue;
            }

            Destroy(child.gameObject);
        }

        foreach (Lobby lobby in lobbyList)
        {
            Transform SingleLobby = Instantiate(_room);
            SingleLobby.transform.SetParent(_roomPosition);
            SingleLobby.gameObject.SetActive(true);
            
            SingleLobbyUI lobbyListSingleUI = SingleLobby.GetComponent<SingleLobbyUI>();
            lobbyListSingleUI.UpdateLobby(lobby);
        }
    } 

    private void Show()
    {
        gameObject.SetActive(true);
    }
}