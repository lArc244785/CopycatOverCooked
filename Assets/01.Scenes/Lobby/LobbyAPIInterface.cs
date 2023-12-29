using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;

using Unity.Services.Lobbies.Models;
using System;
using System.Diagnostics;
using UnityEngine;

public class LobbyAPIInterface
{
    public async Task<Lobby> CreateLobbyAsync(string lobbyname,
                                              int maxPlayer,
                                              bool isprivate,
                                              Dictionary<string, DataObject> lobbyData,
                                              string hostID,
                                              Dictionary<string, PlayerDataObject> hostData)
    {
        CreateLobbyOptions options = new CreateLobbyOptions
        {
            Data = lobbyData,
            IsLocked = true,
            IsPrivate = isprivate,
            Player = new Unity.Services.Lobbies.Models.Player(id: hostID, data: hostData),
        };

        return await LobbyService.Instance.CreateLobbyAsync(lobbyname, maxPlayer, options);
    }
}

public class LobbyServiceFacade
{
    LobbyAPIInterface _interface;
    LocalLobbyUser _localUser;

    public event Action<Lobby> onChanged;

    LobbyServiceFacade(LobbyAPIInterface lobbyAPIInterface)
    {
        _interface = lobbyAPIInterface;
    }

    public void CreateLobby(string lobbyName, int maxPlayers, bool isprivate)
    {
        Task<Lobby>.Run(async () =>
        {
            Lobby result;

            try
            {
                result = await _interface.CreateLobbyAsync(lobbyName, maxPlayers, isprivate, null, AuthenticationService.Instance.PlayerId,
                                                     new Dictionary<string, PlayerDataObject>()
                                                     {
                                                         { "DisplayName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, _localUser.displayName)}
                                                     });
            }

            catch (Exception exception)
            {
                UnityEngine.Debug.Log(exception);
            }

            finally
            {

            }
        });
    }
}