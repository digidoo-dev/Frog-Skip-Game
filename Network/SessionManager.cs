using System;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Multiplayer;
using UnityEngine;


public class SessionManager : MonoBehaviour
{
    public static SessionManager Instance { get; private set; }



    public event EventHandler OnInitializeSuccess;
    public event EventHandler OnInitializeError;




    [SerializeField] private GameObject lobbyManagerPrefab;



    public IHostSession CreatedSession { get; private set; }
    public ISession JoinedSession { get; private set; }



    private void Awake()
    {
        if (Instance != null)
        {
            Debug.Log("There is more than one SessionManager!");
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        DontDestroyOnLoad(gameObject);
    }



    async void Start()
    {
        try
        {
            await UnityServices.InitializeAsync();

            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                Debug.Log($"Sign in anonymously succeeded! Player ID: {AuthenticationService.Instance.PlayerId}");
            }

            OnInitializeSuccess?.Invoke(this, EventArgs.Empty);

        }
        catch (Exception e)
        {
            OnInitializeError?.Invoke(this, new EventArgsCollection.ErrorMessageArgs(e.Message));
        }
    }

    public async void GetPublicLobbies()
    {
        QuerySessionsOptions options = new QuerySessionsOptions() { Count = 20 };

        try
        {
            var publicSessions = await MultiplayerService.Instance.QuerySessionsAsync(options);
            Debug.Log("GetPublicLobbies po await.");
            GameObject.Find("Canvas").GetComponent<PlayOptionsUIController>().DrawListOfLobbies(publicSessions?.Sessions);
        }
        catch (Exception e)
        {
            GameObject.Find("StateManager").GetComponent<PlayOptionsStateManager>().SwitchToError(e.Message);
        }
    }

    public async void StartSessionAsHost(string lobbyName, int maxPlayers)
    {
        var options = new SessionOptions
        {
            Name = lobbyName,
            MaxPlayers = maxPlayers
        }.WithRelayNetwork();

        GameObject.Find("StateManager").GetComponent<PlayOptionsStateManager>().SwitchToConnecting();

        try
        {
            CreatedSession = await MultiplayerService.Instance.CreateSessionAsync(options);
        }
        catch (Exception e)
        {
            GameObject.Find("StateManager").GetComponent<PlayOptionsStateManager>().SwitchToError(e.Message);
            return;
        }

        var instance = Instantiate(lobbyManagerPrefab);
        var instanceNetworkObject = instance.GetComponent<NetworkObject>();
        instanceNetworkObject.Spawn(false);

        NetworkManager.Singleton.SceneManager.LoadScene("Lobby Scene", UnityEngine.SceneManagement.LoadSceneMode.Single);
    }



    public async void JoinSessionByCode(string joinCode)
    {
        GameObject.Find("StateManager").GetComponent<PlayOptionsStateManager>().SwitchToConnecting();

        try
        {
            JoinedSession = await MultiplayerService.Instance.JoinSessionByCodeAsync(joinCode);
        }
        catch (Exception e)
        {
            GameObject.Find("StateManager").GetComponent<PlayOptionsStateManager>().SwitchToError(e.Message);
            return;
        }
    }

    public async void JoinSessionById(string id)
    {
        GameObject.Find("StateManager").GetComponent<PlayOptionsStateManager>().SwitchToConnecting();

        try
        {
            JoinedSession = await MultiplayerService.Instance.JoinSessionByIdAsync(id);
        }
        catch (Exception e)
        {
            GameObject.Find("StateManager").GetComponent<PlayOptionsStateManager>().SwitchToError(e.Message);
            return;
        }
    }



    public async void CreatedSessionStopAcceptingNewPlayers()
    {
        CreatedSession.IsLocked = true;
        await CreatedSession.SavePropertiesAsync();
    }

    public async void CreatedSessionStartAcceptingNewPlayers()
    {
        CreatedSession.IsLocked = false;
        await CreatedSession.SavePropertiesAsync();
    }


    public async void QuitCreatedSession()
    {
        LobbyManager.Instance.DisconnectAllClientsRpc();

        await CreatedSession.DeleteAsync();
        if (RaceManager.Instance != null) Destroy(RaceManager.Instance.gameObject);
        if (LobbyManager.Instance != null) Destroy(LobbyManager.Instance.gameObject);
    }

    public async void QuitJoinedSession()
    {
        LobbyManager.Instance.ServerRemovePlayerFromListRpc(NetworkManager.Singleton.LocalClientId);
        
        try
        {
            await JoinedSession.LeaveAsync();
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }

        if (RaceManager.Instance != null) Destroy(RaceManager.Instance.gameObject);
        if (LobbyManager.Instance != null) Destroy(LobbyManager.Instance.gameObject);
    }
}
