using System.Collections.Generic;
using TMPro;
using Unity.Services.Multiplayer;
using UnityEngine;
using UnityEngine.UI;

public class PlayOptionsUIController : BaseUIController
{
    [SerializeField] private GameObject lobbyListOneLobbyViewPrefab;
    [SerializeField] private PlayOptionsStateManager stateManager;

    [SerializeField] private GameObject lobbiesViewerGroup;
    [SerializeField] private GameObject lobbiesScrollViewContent;
    [SerializeField] private TextMeshProUGUI noLobbiesMessageText;
    [SerializeField] private Button lobbiesRefreshButton;
    [SerializeField] private Button lobbiesCreateLobbyButton;
    [SerializeField] private Button lobbiesJoinLobbyByCodeButton;
    [SerializeField] private Button lobbiesBackButton;

    [SerializeField] private GameObject createLobbyGroup;
    [SerializeField] private TMP_InputField lobbyNameInputField;
    [SerializeField] private Slider maxPlayersSlider;
    [SerializeField] private TextMeshProUGUI maxPlayersText;
    [SerializeField] private Button createLobbyCreateButton;

    [SerializeField] private GameObject joinLobbyByCodeGroup;
    [SerializeField] private TMP_InputField joinCodeInputField;
    [SerializeField] private Button joinLobbyByCodeButton;

    [SerializeField] private GameObject connectingGroup;

    [SerializeField] private GameObject errorGroup;
    [SerializeField] private TextMeshProUGUI errorText;

    private Dictionary<ProgramStates.PlayOptionsStates, GameObject> stateToUIGroupDictionary = new Dictionary<ProgramStates.PlayOptionsStates, GameObject>();
    private List<GameObject> lobbyListViewsList = new List<GameObject>();

    private void Start()
    {
        stateToUIGroupDictionary[ProgramStates.PlayOptionsStates.Lobbies] = lobbiesViewerGroup;
        stateToUIGroupDictionary[ProgramStates.PlayOptionsStates.CreateNewLobby] = createLobbyGroup;
        stateToUIGroupDictionary[ProgramStates.PlayOptionsStates.JoinLobbyByCode] = joinLobbyByCodeGroup;
        stateToUIGroupDictionary[ProgramStates.PlayOptionsStates.Connecting] = connectingGroup;
        stateToUIGroupDictionary[ProgramStates.PlayOptionsStates.Error] = errorGroup;

        SessionManager.Instance.GetPublicLobbies();

        lobbyNameInputField.text = $"{PlayerInfoHolder.FrogName}'s lobby";

        stateManager.OnStateChanged += StateManager_OnStateChanged;

        lobbiesRefreshButton.onClick.AddListener(() =>
        {
            SessionManager.Instance.GetPublicLobbies();
        });

        maxPlayersSlider.onValueChanged.AddListener((value) =>
        {
            maxPlayersText.text = value.ToString();
        });

        createLobbyCreateButton.onClick.AddListener(() =>
        {
            SessionManager.Instance.StartSessionAsHost(lobbyNameInputField.text, (int)maxPlayersSlider.value);
        });

        joinLobbyByCodeButton.onClick.AddListener(() =>
        {
            SessionManager.Instance.JoinSessionByCode(joinCodeInputField.text);
        });
    }

    private void StateManager_OnStateChanged(object sender, System.EventArgs e)
    {
        var args = e as EventArgsCollection.PlayOptionsStateChangeArgs;
        var switchFrom = args.CurrentState;
        var switchTo = args.NewState;

        if (switchTo == ProgramStates.PlayOptionsStates.Error)
        {
            errorText.text = args.ErrorMsg;
        }

        SwitchLayout(stateToUIGroupDictionary[switchFrom], stateToUIGroupDictionary[switchTo]);
    }

    public void DrawListOfLobbies(IList<ISessionInfo> lobbiesList)
    {
        foreach (GameObject view in lobbyListViewsList)
        {
            Destroy(view);
        }

        lobbyListViewsList.Clear();

        if (lobbiesList.Count == 0) 
        {
            noLobbiesMessageText.gameObject.SetActive(true);
            return;
        }

        noLobbiesMessageText.gameObject.SetActive(false);


        for (int i = 0; i < lobbiesList.Count; i++)
        {
            var lobbyInfo = lobbiesList[i];
            float yDisplacement = 100f;
            GameObject view = Instantiate(lobbyListOneLobbyViewPrefab, lobbiesScrollViewContent.transform);
            view.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, -50f - i * yDisplacement, 0);
            view.transform.Find("NameText").GetComponent<TextMeshProUGUI>().text = lobbyInfo.Name;
            view.transform.Find("PlayersNumberText").GetComponent<TextMeshProUGUI>().text = $"{lobbyInfo.MaxPlayers - lobbyInfo.AvailableSlots}/{lobbyInfo.MaxPlayers}";
            view.transform.Find("JoinButton").GetComponent<Button>().onClick.AddListener(() =>
            {
                SessionManager.Instance.JoinSessionById(lobbyInfo.Id);
            });

            lobbyListViewsList.Add(view);
        }
    }
}
