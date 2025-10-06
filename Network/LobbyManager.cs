using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : NetworkBehaviour
{
    public static LobbyManager Instance { get; private set; }

    public event EventHandler OnLobbyChanged;
    public event EventHandler OnPlayerReadinessChanged;



    [SerializeField] private GameObject raceManagerPrefab;



    private Dictionary<ulong, LobbyPlayerInfo> playerClientIdToInfoDictionary = new Dictionary<ulong, LobbyPlayerInfo>();


    [Rpc(SendTo.Server)]
    private void AddNewPlayerToListRpc(ulong playerClientId, string frogName, Color frogColor)
    {
        if (playerClientIdToInfoDictionary.ContainsKey(playerClientId)) return;
        playerClientIdToInfoDictionary[playerClientId] = new LobbyPlayerInfo(frogName, frogColor);

        foreach (ulong id in playerClientIdToInfoDictionary.Keys)
        {
            SyncPlayerListWithClientsRpc(id, playerClientIdToInfoDictionary[id].FrogName, playerClientIdToInfoDictionary[id].FrogColor, playerClientIdToInfoDictionary[id].Readiness); 
        }

        OnLobbyChanged?.Invoke(this, new EventArgsCollection.LobbyPlayerInfoArgs(playerClientIdToInfoDictionary));
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void SyncPlayerListWithClientsRpc(ulong playerClientId, string frogName, Color frogColor, bool readiness)
    {
        if (playerClientIdToInfoDictionary.ContainsKey(playerClientId)) return;
        playerClientIdToInfoDictionary[playerClientId] = new LobbyPlayerInfo(frogName, frogColor, readiness);

        OnLobbyChanged?.Invoke(this, new EventArgsCollection.LobbyPlayerInfoArgs(playerClientIdToInfoDictionary));
    }


    [Rpc(SendTo.Server)]
    public void ServerRemovePlayerFromListRpc(ulong playerClientId)
    {
        if (!playerClientIdToInfoDictionary.ContainsKey(playerClientId)) return;
        playerClientIdToInfoDictionary.Remove(playerClientId);

        ClientsRemovePlayerFromListRpc(playerClientId);

        OnLobbyChanged?.Invoke(this, new EventArgsCollection.LobbyPlayerInfoArgs(playerClientIdToInfoDictionary));
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void ClientsRemovePlayerFromListRpc(ulong playerClientId)
    {
        if (!playerClientIdToInfoDictionary.ContainsKey(playerClientId)) return;
        playerClientIdToInfoDictionary.Remove(playerClientId);

        OnLobbyChanged?.Invoke(this, new EventArgsCollection.LobbyPlayerInfoArgs(playerClientIdToInfoDictionary));
    }


    [Rpc(SendTo.Server)]
    public void ChangeReadinessRpc(ulong playerId)
    {
        if (!playerClientIdToInfoDictionary.ContainsKey(playerId)) return;
        playerClientIdToInfoDictionary[playerId].SwitchReadiness();

        SyncReadinessWithClientsRpc(playerId, playerClientIdToInfoDictionary[playerId].Readiness);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void SyncReadinessWithClientsRpc(ulong playerId, bool readiness)
    {
        if (!playerClientIdToInfoDictionary.ContainsKey(playerId)) return;
        playerClientIdToInfoDictionary[playerId].SetReadiness(readiness);

        OnPlayerReadinessChanged?.Invoke(this, new EventArgsCollection.LobbyPlayerIdAndReadinessArgs(playerId, readiness));
    }

    [Rpc(SendTo.Server)]
    public void LaunchServerRejoiningLobbyProcessRpc()
    {
        foreach(ulong id in playerClientIdToInfoDictionary.Keys)
        {
            playerClientIdToInfoDictionary[id].SetReadiness(false);
            SyncReadinessWithClientsRpc(id, false);
        }
        LaunchClientsRejoiningLobbyProcessRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void LaunchClientsRejoiningLobbyProcessRpc()
    {
        StartCoroutine(WaitForLobbyUIThenDrawIt());
    }


 
    [Rpc(SendTo.ClientsAndHost)]
    public void DisconnectAllClientsRpc()
    {
        if (IsServer) return;

        SessionManager.Instance.QuitJoinedSession();
        SceneManager.LoadScene("Host Disconnected Message Scene");
    }





    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There is more than one LobbyManager!");
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        AddNewPlayerToListRpc(NetworkManager.Singleton.LocalClientId, PlayerInfoHolder.FrogName, PlayerInfoHolder.FrogColor);

        if (IsServer)
        {
            StartCoroutine(WaitForLobbyUIThenDrawIt());
        }
    }









    private IEnumerator WaitForLobbyUIThenDrawIt()
    {
        bool lobbyUIReady = false;

        while (!lobbyUIReady)
        {
            if (SceneManager.GetActiveScene().name == "Lobby Scene")
            {
                if (GameObject.Find("Canvas").GetComponent<LobbyUIController>().hasStarted)
                {
                    lobbyUIReady = true;
                    OnLobbyChanged?.Invoke(this, new EventArgsCollection.LobbyPlayerInfoArgs(playerClientIdToInfoDictionary));

                }
            }
            yield return null;
        }
    }


    public List<ulong> GetListOfPlayerIDs()
    {
        List<ulong> list = new List<ulong>();
        foreach (ulong id in playerClientIdToInfoDictionary.Keys) { list.Add(id); }
        return list;
    }

    public string GetFrogName(ulong playerClientId)
    {
        return playerClientIdToInfoDictionary[playerClientId].FrogName;
    }

    public Color GetFrogColor(ulong playerClientId)
    {
        return playerClientIdToInfoDictionary[playerClientId].FrogColor;
    }

    public void StartRace()
    {
        if (!IsServer) return;
        var raceManager = Instantiate(raceManagerPrefab).GetComponent<NetworkObject>();
        raceManager.Spawn(false);

        NetworkManager.Singleton.SceneManager.LoadScene("Race Scene", LoadSceneMode.Single);
    }

    
}
