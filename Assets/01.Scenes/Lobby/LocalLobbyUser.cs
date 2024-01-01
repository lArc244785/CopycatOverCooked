using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalLobbyUser
{
    public bool isHost
    {
        get
        {
            return userData.isHost;
        }

        set
        {
            if (userData.isHost == value)
            {
                return;
            }

            _userData.isHost = value;
            onChanged?.Invoke(this);
        }
    }

    public string displayName
    {
        get
        {
            return userData.displayName;
        }

        set
        {
            if (userData.displayName == value)
            {
                return;
            }

            _userData.displayName = value;
            onChanged?.Invoke(this);
        }
    }

    public string id
    {
        get
        {
            return userData.id;
        }

        set
        {
            if (userData.id == value)
            {
                return;
            }

            _userData.id = value;
            onChanged?.Invoke(this);
        }
    }

    public struct UserData
    {
        public bool isHost;
        public string displayName;
        public string id;

        public UserData(bool ishost, string displayname, string id)
        {
            this.isHost = ishost;
            this.displayName = displayname;
            this.id = id;
        }
    }
    public UserData userData
    { 
        get
        {
            return _userData;
        }

        set
        {
            _userData = value;
            onChanged?.Invoke(this);
        }
    }
    private UserData _userData;

    public event Action<LocalLobbyUser> onChanged;
}