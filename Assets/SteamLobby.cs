using Mirror;
using Steamworks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SteamLobby : MonoBehaviour
{
    [SerializeField] private GameObject hostButton = null;

    private const string HostAddressKey = "HostAddress";

    private NetworkManager networkManager;
    public static CSteamID LobbyID { get; private set; }

    protected Callback<LobbyCreated_t> lobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> joinRequested;
    protected Callback<LobbyEnter_t> lobbyEntered;


    private void Start()
    {
        networkManager = GetComponent<NetworkManager>();
        hostButton = GetComponentInChildren<Button>().gameObject;
        if(!SteamManager.Initialized) { return; }

        lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        joinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnJoinRequest);
        lobbyEntered = Callback<LobbyEnter_t>.Create(LobbyEntered);

    }

    public void HostLobby()
    {
        hostButton.SetActive(false);
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, networkManager.maxConnections);
    }
    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        if(callback.m_eResult != EResult.k_EResultOK)
        {
            hostButton.SetActive(true);
            return;
        }

        LobbyID = new CSteamID(callback.m_ulSteamIDLobby);

        networkManager.StartHost();

        SteamMatchmaking.SetLobbyData(
            LobbyID,
            HostAddressKey,
            SteamUser.GetSteamID().ToString());

    }

    private void OnJoinRequest(GameLobbyJoinRequested_t callback)
    {
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    private void LobbyEntered(LobbyEnter_t callback)
    {
        if(NetworkServer.active) { return; }

        string hostAddress = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey);

        networkManager.networkAddress = hostAddress;
        networkManager.StartClient();


        hostButton.SetActive(false);
    }

    public void StartGame()
    {
        SceneManager.LoadScene(1, LoadSceneMode.Single);
    }
}
