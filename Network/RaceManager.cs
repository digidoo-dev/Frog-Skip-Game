using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class RaceManager : NetworkBehaviour
{
    public static RaceManager Instance { get; private set; }

    [SerializeField] private GameObject playerObjectPrefab;






    private enum RaceState
    {
        Initialization,
        CountdownToStart,
        RaceInProgress,
        CountdownToFinish,
        RaceOver
    }
    private RaceState raceState;


    private RaceUIController raceUIController;

    private Dictionary<ulong, RaceStatistics> playerIdToRaceStatisticsDictionary = new Dictionary<ulong, RaceStatistics>();
    private int numberOfPlayersWhoFinishedRace = 0;

    private Dictionary<ulong, bool> playerIdToIsLoadedInToRaceScene = new Dictionary<ulong, bool>();

    private Dictionary<ulong, PlayerController> playerIdToPlayerControllerDictionary = new Dictionary<ulong, PlayerController>();


    private NetworkVariable<float> countdownToStart = new NetworkVariable<float>(3f);
    private NetworkVariable<float> raceTimer = new NetworkVariable<float>(0f);
    private NetworkVariable<float> countdownToFinish = new NetworkVariable<float>(20f);



    [Rpc(SendTo.ClientsAndHost)]
    private void ClientsLaunchCountdownToStartUiRpc()
    {
        raceUIController = GameObject.Find("Canvas").GetComponent<RaceUIController>();

        raceUIController.ShowCountdownToStart();
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void ClientsLaunchRaceTimerUiRpc()
    {
        raceUIController.ShowRaceTimer();
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void ClientsLaunchCountdownToFinishUiRpc()
    {
        raceUIController.ShowCountdownToFinish();
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void ClientsLaunchEndOfRaceUiRpc()
    {
        raceUIController.ShowRaceOverMessage();
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void ClientsSaveRaceStatisticsRpc(ulong playerId, bool finishedRace, float time, int spot)
    {
        if (!playerIdToRaceStatisticsDictionary.ContainsKey(playerId))
        {
            playerIdToRaceStatisticsDictionary[playerId] = new RaceStatistics();
        }
        
        
        if (finishedRace)
        {
            playerIdToRaceStatisticsDictionary[playerId].FinishedRace(time, spot);
        }

    }











    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There is more than one RaceManager!");
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        List<ulong> playerIds = LobbyManager.Instance.GetListOfPlayerIDs();
        foreach (ulong playerId in playerIds)
        {
            playerIdToRaceStatisticsDictionary[playerId] = new RaceStatistics();

            if (IsServer) playerIdToIsLoadedInToRaceScene[playerId] = false;
        }

        if (IsServer)
        {
            NetworkManager.Singleton.SceneManager.OnLoadComplete += SceneManager_OnLoadComplete;
        }
    }

    private new void OnDestroy()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.SceneManager.OnLoadComplete -= SceneManager_OnLoadComplete;
        }
    }


    private void SceneManager_OnLoadComplete(ulong clientId, string sceneName, UnityEngine.SceneManagement.LoadSceneMode loadSceneMode)
    {
        if (sceneName != "Race Scene") return;
        playerIdToIsLoadedInToRaceScene[clientId] = true;

        IfAllPlayersAreLoadedInSpawnPlayerObjects();
    }












    private void IfAllPlayersAreLoadedInSpawnPlayerObjects()
    {
        bool allLoadedIn = true;
        foreach (bool loadedIn in  playerIdToIsLoadedInToRaceScene.Values)
        {
            if (!loadedIn) allLoadedIn = false;
        }

        if (!allLoadedIn) return;

        SpawnPointsHolder spawnsHolder = GameObject.Find("SpawnPoints").GetComponent<SpawnPointsHolder>();

        foreach (ulong id in playerIdToRaceStatisticsDictionary.Keys)
        {
            var playerObject = Instantiate(playerObjectPrefab);
            var playerNetworkObject = playerObject.GetComponent<NetworkObject>();
            playerNetworkObject.SpawnAsPlayerObject(id, true);

            var playerController = playerObject.GetComponent<PlayerController>();
            playerIdToPlayerControllerDictionary[id] = playerController;

            playerObject.transform.position = spawnsHolder.GetRandomUnoccupiedSpawnPositionAndMarkItAsOccupied(playerIdToRaceStatisticsDictionary.Keys.Count);

            playerController.SetupPlayerControllerRpc();
            playerController.SetFrogColorAndDeactivateScriptIfNotOwnedRpc();
        }

        StartCountdownToStart();
    }





    private void StartCountdownToStart()
    {
        raceState = RaceState.CountdownToStart;
        ClientsLaunchCountdownToStartUiRpc();
        StartCoroutine(UpdateCountdownToStartEvery(.5f));
    }

    private void StartRace()
    {
        raceState = RaceState.RaceInProgress;
        ClientsLaunchRaceTimerUiRpc();
        StartCoroutine(UpdateRaceTimerEvery(.1f));

        foreach(PlayerController pc in playerIdToPlayerControllerDictionary.Values)
        {
            pc.MakeMasterControlsRpc(true);
        }
    }

    private void StartCountdownToFinish()
    {
        raceState = RaceState.CountdownToFinish;
        ClientsLaunchCountdownToFinishUiRpc();
        StartCoroutine(UpdateCountdownToFinishEvery(.1f));
    }

    private void FinishRace()
    {
        raceState = RaceState.RaceOver;
        ClientsLaunchEndOfRaceUiRpc();
        

        foreach (PlayerController pc in playerIdToPlayerControllerDictionary.Values)
        {
            pc.MakeMasterControlsRpc(false);
        }
        foreach (ulong id in playerIdToRaceStatisticsDictionary.Keys)
        {
            RaceStatistics raceStats = playerIdToRaceStatisticsDictionary[id];
            ClientsSaveRaceStatisticsRpc(id, raceStats.RaceFinished, raceStats.RaceTime, raceStats.FinishedAtPlace);
        }

        StartCoroutine(LoadStatisticsPageAfter(3f));
    }


    private IEnumerator UpdateCountdownToStartEvery(float seconds)
    {
        while (raceState == RaceState.CountdownToStart)
        {
            yield return new WaitForSeconds(seconds);
            countdownToStart.Value -= seconds;

            if (countdownToStart.Value <= 0f) StartRace();
        }
    }

    private IEnumerator UpdateRaceTimerEvery(float seconds)
    {
        while (raceState == RaceState.RaceInProgress || raceState == RaceState.CountdownToFinish)
        {
            yield return new WaitForSeconds(seconds);
            raceTimer.Value += seconds;
        }
    }

    private IEnumerator UpdateCountdownToFinishEvery(float seconds)
    {
        while (raceState == RaceState.CountdownToFinish)
        {
            yield return new WaitForSeconds(seconds);
            countdownToFinish.Value -= seconds;

            if (countdownToFinish.Value <= 0f) FinishRace();
        }
    }

    private IEnumerator LoadStatisticsPageAfter(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        NetworkManager.Singleton.SceneManager.LoadScene("Statistics Scene", UnityEngine.SceneManagement.LoadSceneMode.Single);
    }








    public void PlayerFinishedRace(ulong id)
    {
        if (playerIdToRaceStatisticsDictionary[id].RaceFinished) return;

        playerIdToRaceStatisticsDictionary[id].FinishedRace(raceTimer.Value, numberOfPlayersWhoFinishedRace + 1);
        numberOfPlayersWhoFinishedRace += 1;

        playerIdToPlayerControllerDictionary[id].MakeMasterControlsRpc(false);

        if (playerIdToRaceStatisticsDictionary.Keys.Count == numberOfPlayersWhoFinishedRace)
        {
            FinishRace();
        }
        else if (numberOfPlayersWhoFinishedRace == 1)
        {
            StartCountdownToFinish();
        }
    }





    public float GetCountdownToStartValue()
    {
        return countdownToStart.Value;
    }
    public float GetRaceTimerValue()
    {
        return raceTimer.Value;
    }
    public float GetCountdownToFinishValue()
    {
        return countdownToFinish.Value;
    }

    public Dictionary<ulong, RaceStatistics> GetRaceStatistics()
    {
        return playerIdToRaceStatisticsDictionary;
    }
}
