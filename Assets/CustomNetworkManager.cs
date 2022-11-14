using Mirror;
using Steamworks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomNetworkManager : NetworkManager
{

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);
        CSteamID steamID = SteamMatchmaking.GetLobbyMemberByIndex(SteamLobby.LobbyID, numPlayers - 1);
        var playerInfoDisplay = conn.identity.GetComponent<PlayerInfoDisplay>();

        /*
         * Disabled since steam cuts off connections when doing this. I think its with the same ID creating multiple players.
        playerInfoDisplay.SetSteamId(steamID.m_SteamID);
        */

    }
}
