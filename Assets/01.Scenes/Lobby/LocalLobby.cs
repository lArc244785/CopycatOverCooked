using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LocalLobby
{    
    public string code
    {
        get
        {
            return lobbyData.lobbycode;
        }

        set
        {
            if (lobbyData.lobbycode == value)
            {
                return;
            }

            lobbyData.lobbycode = value;
            onChanged?.Invoke(this);
        }
    }

    public string relayJoinCode
    {
        get
        {
            return lobbyData.relayJoinCode;
        }

        set
        {
            if (lobbyData.relayJoinCode == value)
            {
                return;
            }

            lobbyData.relayJoinCode = value;
            onChanged?.Invoke(this);
        }
    }

    public string name
    {
        get
        {
            return lobbyData.name;
        }

        set
        {
            if (lobbyData.name == value)
            {
                return;
            }

            lobbyData.name = value;
            onChanged?.Invoke(this);
        }
    }

    public bool isPrivate
    {
        get
        {
            return lobbyData.isPrivate;
        }

        set
        {
            if (lobbyData.isPrivate == value)
            {
                return;
            }

            lobbyData.isPrivate = value;
            onChanged?.Invoke(this);
        }
    }

    public struct LobbyData
    {
        public int numberofPlayer;
        public string lobbycode;
        public string relayJoinCode;
        public string name;
        public bool isPrivate;

        public LobbyData(int numberofplayer, string code, string relayjoincode, string name, bool isPrivate)
        {
            this.numberofPlayer = numberofplayer;
            this.lobbycode = code;
            this.relayJoinCode = relayjoincode;
            this.name = name;
            this.isPrivate = isPrivate;
        }
    }
    public LobbyData lobbyData;

    public Dictionary<string, LocalLobbyUser> localLobbyUsers = new Dictionary<string, LocalLobbyUser>();
    
    public event Action<LocalLobby> onChanged;
    public event Action onNumberOfPeopleChanged;

    public void AddUser(LocalLobbyUser newuser)
    {
        if(!localLobbyUsers.ContainsKey(newuser.id))
        {
            localLobbyUsers.Add(newuser.id, newuser);
            onChanged?.Invoke(this);
        }
    }

    public void RemoveUser(LocalLobbyUser user) 
    {
        if (!localLobbyUsers.ContainsKey(user.id))
        {
            localLobbyUsers.Remove(user.id);
            onChanged?.Invoke(this);
        }
    }
}

/*
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LocalLobby
{
    public string id
    {
        get
        {
            return lobbyData.id;
        }

        set
        {
            if (lobbyData.id == value)
            {
                return;
            }

            lobbyData.id = value;
            onChanged?.Invoke(this);
        }
    }

    public string code
    {
        get
        {
            return lobbyData.lobbycode;
        }

        set
        {
            if (lobbyData.lobbycode == value)
            {
                return;
            }

            lobbyData.lobbycode = value;
            onChanged?.Invoke(this);
        }
    }

    public string relayJoinCode
    {
        get
        {
            return lobbyData.relayJoinCode;
        }

        set
        {
            if (lobbyData.relayJoinCode == value)
            {
                return;
            }

            lobbyData.relayJoinCode = value;
            onChanged?.Invoke(this);
        }
    }

    public string name
    {
        get
        {
            return lobbyData.name;
        }

        set
        {
            if (lobbyData.name == value)
            {
                return;
            }

            lobbyData.name = value;
            onChanged?.Invoke(this);
        }
    }

    public bool isPrivate
    {
        get
        {
            return lobbyData.isPrivate;
        }

        set
        {
            if (lobbyData.isPrivate == value)
            {
                return;
            }

            lobbyData.isPrivate = value;
            onChanged?.Invoke(this);
        }
    }

    public int MaxPlayers
    {
        get
        {
            return lobbyData.MaxPlayers;
        }

        set
        {
            if (lobbyData.MaxPlayers == value)
            {
                return;
            }

            lobbyData.MaxPlayers = value;
            onChanged?.Invoke(this);
        }
    }

    public struct LobbyData
    {
        public string id;
        public string lobbycode;
        public string relayJoinCode;
        public string name;
        public bool isPrivate;
        public int MaxPlayers;

        public LobbyData(string id, string code, string relayjoincode, string name, bool isPrivate, int maxplayercount = 4)
        {
            this.id = id;
            this.lobbycode = code;
            this.relayJoinCode = relayjoincode;
            this.name = name;
            this.isPrivate = isPrivate;
            this.MaxPlayers = maxplayercount;
        }
    }
    public LobbyData lobbyData;

    public Dictionary<string, LocalLobbyUser> _localLobbyUsers = new Dictionary<string, LocalLobbyUser>();
    
    public event Action<LocalLobby> onChanged;

    public static LocalLobby Create(Lobby lobby)
    {
        LocalLobby locallobby = new LocalLobby();

        locallobby.ApplyRemoteData(lobby);

        return locallobby;
    }

    public void AddUser(LocalLobbyUser newuser)
    {
        if(!_localLobbyUsers.ContainsKey(newuser.id))
        {
            _localLobbyUsers.Add(newuser.id, newuser);
            onChanged?.Invoke(this);
        }
    }

    public void RemoveUser(LocalLobbyUser user) 
    {
        if (!_localLobbyUsers.ContainsKey(user.id))
        {
            _localLobbyUsers.Remove(user.id);
            onChanged?.Invoke(this);
        }
    }

    public void ApplyRemoteData(Lobby lobby)
    {
        LobbyData tmpData = new LobbyData();
        tmpData.id = lobby.Id;
        tmpData.lobbycode = lobby.LobbyCode;

        if (lobby.Data != null)
        {
            tmpData.relayJoinCode = lobby.Data.ContainsKey("RelayJoinCode") ? lobby.Data["RelayJoinCode"].Value : null;
        }

        tmpData.isPrivate = lobby.IsPrivate;
        tmpData.name = lobby.Name;
        tmpData.MaxPlayers = lobby.MaxPlayers;

        _localLobbyUsers.Clear();

        foreach (Unity.Services.Lobbies.Models.Player player in lobby.Players)
        {
            if (player.Data != null)
            {
                if (!_localLobbyUsers.ContainsKey(player.Id))
                {
                    _localLobbyUsers.Add(player.Id, new LocalLobbyUser()
                    {
                        userData = new LocalLobbyUser.UserData(ishost : lobby.HostId == player.Id,
                                                               displayname : player.Data != null && player.Data.ContainsKey("DisplayName") ? player.Data["DisplayName"].Value : string.Empty,
                                                               id : player.Id)
                    });
                }
            }
        }

        lobbyData = tmpData;
        onChanged?.Invoke(this);
    }
}*/