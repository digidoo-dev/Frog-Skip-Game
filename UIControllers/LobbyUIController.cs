using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyUIController : MonoBehaviour
{
    [HideInInspector] public bool hasStarted = false;

    [SerializeField] private GameObject lobbyPlayerCardPrefab;
    [SerializeField] private Sprite readySprite;
    [SerializeField] private Sprite notReadySprite;

    [SerializeField] private TextMeshProUGUI lobbyNameText;
    [SerializeField] private TextMeshProUGUI joinCodeText;
    [SerializeField] private Button backButton;

    [SerializeField] private LevelChooserController levelChooserController;

    [SerializeField] private Button readyButton;
    [SerializeField] private Button startGameButton;

    [SerializeField] private List<Transform> spawnPoints;
    private List<bool> spawnPointOccupied = new List<bool>();

    private Dictionary<ulong, GameObject> playerCardsDictionary = new Dictionary<ulong, GameObject>();

    private Dictionary<ulong, LobbyPlayerInfo> playerIdToInfoDictionary = new Dictionary<ulong, LobbyPlayerInfo>();



    void Start()
    {
        foreach (Transform t in spawnPoints)
        {
            spawnPointOccupied.Add(false);
        }

        if (NetworkManager.Singleton.IsHost)
        {
            var createdSession = SessionManager.Instance.CreatedSession;
            lobbyNameText.text = createdSession.Name;
            joinCodeText.text = createdSession.Code;
        }
        else if (NetworkManager.Singleton.IsClient)
        {
            var joinedSession = SessionManager.Instance.JoinedSession;
            lobbyNameText.text = joinedSession.Name;
            joinCodeText.text = joinedSession.Code;
            startGameButton.gameObject.SetActive(false);
            levelChooserController.HideSelectionButtons();
        }

        LobbyManager.Instance.OnLobbyChanged += LobbyManager_OnLobbyChanged;
        LobbyManager.Instance.OnPlayerReadinessChanged += LobbyManager_OnPlayerReadinessChanged;
        LobbyManager.Instance.OnSelectedLevelChanged += LobbyManager_OnSelectedLevelChanged;

        hasStarted = true;



        backButton.onClick.AddListener(() =>
        {
            if (!NetworkManager.Singleton.IsServer)
            {
                SessionManager.Instance.QuitJoinedSession();
                SceneManager.LoadScene("Play Options Scene");
            }
            else
            {
                SessionManager.Instance.QuitCreatedSession();
                SceneManager.LoadScene("Play Options Scene");
            }
        });

        readyButton.onClick.AddListener(() =>
        {
            LobbyManager.Instance.ChangeReadinessRpc(NetworkManager.Singleton.LocalClientId);
            StartCoroutine(DisableReadinessChangeButtonFor(2f));
            Sprite sprite = readyButton.GetComponent<Image>().sprite;
            readyButton.GetComponent<Image>().sprite = sprite == readySprite ? notReadySprite : readySprite;
        });

        startGameButton.onClick.AddListener(() =>
        {
            startGameButton.interactable = false;
            SessionManager.Instance.CreatedSessionStopAcceptingNewPlayers();
            LobbyManager.Instance.StartRace();
        });
    }

 
    private void OnDestroy()
    {
        LobbyManager.Instance.OnLobbyChanged -= LobbyManager_OnLobbyChanged;
        LobbyManager.Instance.OnPlayerReadinessChanged -= LobbyManager_OnPlayerReadinessChanged;
        LobbyManager.Instance.OnSelectedLevelChanged -= LobbyManager_OnSelectedLevelChanged;
    }




    private void LobbyManager_OnSelectedLevelChanged(object sender, System.EventArgs e)
    {
        int levelIndex = (e as EventArgsCollection.IntegerArgs).Value;
        levelChooserController.SetToLevel(levelIndex);
    }


    private void LobbyManager_OnPlayerReadinessChanged(object sender, System.EventArgs e)
    {
        var args = e as EventArgsCollection.LobbyPlayerIdAndReadinessArgs;
        var card = playerCardsDictionary[args.Id];
        card.transform.Find("NameAndReadiness").Find("Readiness").GetComponent<Image>().sprite = args.Readiness ? readySprite : notReadySprite;

        playerIdToInfoDictionary[args.Id].SetReadiness(args.Readiness);

        CheckIfAllPlayersAreReady();
    }

    private void LobbyManager_OnLobbyChanged(object sender, System.EventArgs e)
    {
        var args = e as EventArgsCollection.LobbyPlayerInfoArgs;
        playerIdToInfoDictionary = args.ClientIdToInfoDictionary;
        
        RedrawLobbyPlayers();

        CheckIfAllPlayersAreReady();
    }







    private void CheckIfAllPlayersAreReady()
    {
        bool allReady = true;
        foreach (ulong id in playerIdToInfoDictionary.Keys)
        {
            if (playerIdToInfoDictionary[id].Readiness == false) allReady = false;
        }

        if (startGameButton == null) startGameButton = gameObject.transform.Find("StartGameButton").GetComponent<Button>();

        if (allReady && NetworkManager.Singleton.IsHost)
        {
            startGameButton.interactable = true;
        }
        else
        {
            startGameButton.interactable = false;
        }
    }


    private void RedrawLobbyPlayers()
    {
        foreach (var card in playerCardsDictionary.Values) Destroy(card);
        playerCardsDictionary.Clear();

        for (int i = 0; i < spawnPointOccupied.Count; i++) spawnPointOccupied[i] = false;

        foreach (ulong playerId in playerIdToInfoDictionary.Keys)
        {
            int indexOfEmptySpawnPoint = 0;
            foreach (bool occupied in spawnPointOccupied)
            {
                if (!occupied) break;
                indexOfEmptySpawnPoint++;
            }
            var playerCard = Instantiate(lobbyPlayerCardPrefab, spawnPoints[indexOfEmptySpawnPoint]);
            spawnPointOccupied[indexOfEmptySpawnPoint] = true;
            playerCard.transform.Find("FrogVisual").Find("Body").GetComponent<Image>().color = playerIdToInfoDictionary[playerId].FrogColor;
            playerCard.transform.Find("NameAndReadiness").Find("NameText").GetComponent<TextMeshProUGUI>().text = playerIdToInfoDictionary[playerId].FrogName;
            playerCard.transform.Find("NameAndReadiness").Find("Readiness").GetComponent<Image>().sprite = playerIdToInfoDictionary[playerId].Readiness ? readySprite : notReadySprite;

            playerCardsDictionary[playerId] = playerCard;
        }
    }


    private IEnumerator DisableReadinessChangeButtonFor(float seconds)
    {
        readyButton.interactable = false;
        yield return new WaitForSeconds(seconds);
        readyButton.interactable = true;
    }
}
