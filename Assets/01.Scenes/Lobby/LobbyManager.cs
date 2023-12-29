using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;
using static LocalLobby;
using static UnityEngine.UIElements.UxmlAttributeDescription;

public class LobbyManager
{
    private string _playerName = null;
    private Lobby joinedLobby;
    public LocalLobbyUser _localLobbyUser;
    private LobbyAPIInterface _api;
    public LocalLobby _localLobby;
    private float heartbeatTimer;
    private float lobbyPollTimer;

    public event Action onLobbyCreated;
    public event Action<ILobbyChanges> OnLobbyChanged;
    public event EventHandler<LobbyEventArgs> OnJoinedLobby;
    public event EventHandler<OnLobbyListChangedEventArgs> OnLobbyListChanged;
    public event EventHandler<LobbyEventArgs> OnJoinedLobbyUpdate;
    private ILobbyEvents _lobbyEvents;
    public static LobbyManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new LobbyManager();
            }
            return _instance;
        }
    }
    private static LobbyManager _instance;

    public class LobbyEventArgs : EventArgs
    {
        public Lobby lobby;
    }
    public class OnLobbyListChangedEventArgs : EventArgs
    {
        public List<Lobby> lobbyList;
    }

    public async void Authenticate(string playerName)
    {
        this._playerName = playerName;

        InitializationOptions initializationOptions = new InitializationOptions();
        initializationOptions.SetProfile(playerName);

        await UnityServices.InitializeAsync(initializationOptions);

        try
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();

            Debug.Log("Sign in anonymously succeeded!");
            Debug.Log($"PlayerID: {AuthenticationService.Instance.PlayerId}");

            _localLobbyUser = new LocalLobbyUser();

            _localLobbyUser.userData = new LocalLobbyUser.UserData(false, playerName, AuthenticationService.Instance.PlayerId);

            RefreshLobbyList();
        }

        catch (AuthenticationException ex)
        {
            Debug.LogException(ex);
        }

        catch (RequestFailedException ex)
        {
            Debug.LogException(ex);
        }
    }

    public bool IsLobbyHost()
    {
        return joinedLobby != null && joinedLobby.HostId == AuthenticationService.Instance.PlayerId;
    }

    private bool IsPlayerInLobby()
    {
        if (joinedLobby != null && joinedLobby.Players != null)
        {
            foreach (Unity.Services.Lobbies.Models.Player player in joinedLobby.Players)
            {
                if (player.Id == AuthenticationService.Instance.PlayerId)
                {
                    return true;
                }
            }
        }

        return false;
    }


    public async void HandleLobbyHeartbeat()
    {
        if (IsLobbyHost())
        {
            Debug.Log(IsLobbyHost());

            heartbeatTimer -= Time.deltaTime;

            if (heartbeatTimer < 0f)
            {
                float heartbeatTimerMax = 15f;
                heartbeatTimer = heartbeatTimerMax;

                Debug.Log("Heartbeat");
                await LobbyService.Instance.SendHeartbeatPingAsync(joinedLobby.Id);
            }
        }
    }

    public async void HandleLobbyPolling()
    {
        if (joinedLobby != null)
        {
            lobbyPollTimer -= Time.deltaTime;
            if (lobbyPollTimer < 0f)
            {
                float lobbyPollTimerMax = 1.1f;
                lobbyPollTimer = lobbyPollTimerMax;

                joinedLobby = await LobbyService.Instance.GetLobbyAsync(joinedLobby.Id);

                OnJoinedLobbyUpdate?.Invoke(this, new LobbyEventArgs { lobby = joinedLobby });

                if (!IsPlayerInLobby())
                {
                    // Player was kicked out of this lobby
                    Debug.Log("Kicked from Lobby!");

                    //OnKickedFromLobby?.Invoke(this, new LobbyEventArgs { lobby = joinedLobby });

                    joinedLobby = null;
                }
            }
        }
    }

    public  async void CreateLobby(string lobbyName, int maxPlayers, bool isPrivate, string password = "")
    {
        try
        {
            Dictionary<string, DataObject> lobbyData = new Dictionary<string, DataObject>
                                                       {
                                                           {
                                                               "lobbyData", new DataObject(DataObject.VisibilityOptions.Public, "")
                                                           }
                                                       };

            Dictionary<string, PlayerDataObject> hostData = new Dictionary<string, PlayerDataObject>
                                                            {
                                                                {
                                                                    "playerData", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, _localLobbyUser.displayName)
                                                                }
                                                            };

            CreateLobbyOptions options = new CreateLobbyOptions
            {
                Data = lobbyData,
                IsLocked = false,
                IsPrivate = isPrivate,
                Player = new Unity.Services.Lobbies.Models.Player(id: _localLobbyUser.id, data: hostData),
            };

            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, 4, options);

            joinedLobby = lobby;
            _localLobbyUser.isHost = true;

            _localLobby = new LocalLobby();
            
            _localLobby.AddUser(_localLobbyUser);

            await SubscribeLobbyAsync(lobby.Id);
            OnJoinedLobby?.Invoke(this, new LobbyEventArgs { lobby = lobby });
    }

        catch (Exception except) 
        {
            Debug.Log(except + "땜에 안 된다, 코드 좀 제대로 짜라 ㅄ아");
        }
    }

    public async Task SubscribeLobbyAsync(string lobbyID)
    {
        LobbyEventCallbacks callbacks = new LobbyEventCallbacks();
        callbacks.LobbyChanged += OnLobbyChanged;
        _lobbyEvents = await LobbyService.Instance.SubscribeToLobbyEventsAsync(lobbyID, callbacks);
        OnLobbyChanged += LobbyValueChanged;
    }

    public async Task UnsubscribeLobbyAsync()
    {
        await _lobbyEvents.UnsubscribeAsync();
        OnLobbyChanged -= LobbyValueChanged;
    }

    public Lobby GetJoinedLobby()
    {
        return joinedLobby;
    }

    public async void RefreshLobbyList()
    {
        try
        {
            QueryLobbiesOptions options = new QueryLobbiesOptions();

            options.Filters = new List<QueryFilter> 
            {
                new QueryFilter(
                                field: QueryFilter.FieldOptions.AvailableSlots,
                                op: QueryFilter.OpOptions.GT,
                                value: "0"
                                )
            };

            options.Order = new List<QueryOrder>
            {
                new QueryOrder(
                               asc: false,
                               field: QueryOrder.FieldOptions.Created
                               )
            };

            QueryResponse lobbyListQueryResponse = await Lobbies.Instance.QueryLobbiesAsync();

            Debug.Log("Lobby Count : " + lobbyListQueryResponse.Results.Count);
            OnLobbyListChanged?.Invoke(this, new OnLobbyListChangedEventArgs { lobbyList = lobbyListQueryResponse.Results });
        }

        catch (Exception except)
        {
            Debug.Log(except + "땜에 안 된다, 코드 좀 제대로 짜라 ㅄ아");
        }
    }

    public async void ListLobby()
    {
        try
        {
            QueryResponse response = await Lobbies.Instance.QueryLobbiesAsync();
        }

        catch (Exception except)
        {
            Debug.Log(except + "땜에 안 된다, 코드 좀 제대로 짜라 ㅄ아");
        }
    }

    public async void JoinLobby(Lobby lobby)
    {
        try
        {
            Unity.Services.Lobbies.Models.Player player = new Unity.Services.Lobbies.Models.Player(AuthenticationService.Instance.PlayerId, null, new Dictionary<string, PlayerDataObject>
                                                            {
                                                                {
                                                                    "playerData", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, _localLobbyUser.displayName)
                                                                }
                                                            });
        
            Debug.Log(player.Id);

            joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobby.Id, new JoinLobbyByIdOptions
            {
                Player = player
            });

            //_users.AddUser(_user);
            await SubscribeLobbyAsync(lobby.Id);
            OnJoinedLobby?.Invoke(this, new LobbyEventArgs { lobby = lobby });
        }

        catch (Exception except)
        {
            Debug.Log(except);
        }
    }

    public void LobbyValueChanged(ILobbyChanges lobbyChanges)
    {
            _localLobby.localLobbyUsers = new Dictionary<string, LocalLobbyUser>
                                          (lobbyChanges.PlayerJoined.Value.Select(x => new KeyValuePair<string, LocalLobbyUser>
                                                                                       (x.Player.Id, new LocalLobbyUser
                                                                                       {
                                                                                           id = x.Player.Id,
                                                                                           displayName = x.Player.Data["playerName"].Value,
                                                                                           userData = new LocalLobbyUser.UserData
                                                                                           {
                                                                                               isHost = x.Player.Id == lobbyChanges.HostId.Value,
                                                                                               id = x.Player.Id,
                                                                                               displayName = x.Player.Data["playerName"].Value
                                                                                           }
                                                                                       })));;
    }
}