using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StatisticsUIController : BaseUIController
{
    [SerializeField] private GameObject statisticsPlayerCardPrefab;

    [SerializeField] private TextMeshProUGUI CountdownText;
    [SerializeField] private Button exitInsteadButton;

    [SerializeField] private List<RectTransform> positionsList;
    private List<bool> positionTakenList = new List<bool>();

    private Dictionary<ulong, RaceStatistics> idToRaceStatisticsDictionary = new Dictionary<ulong, RaceStatistics>();

    private float countdown = 10f;

    private void Start()
    {
        foreach (var t in positionsList) positionTakenList.Add(false);

        idToRaceStatisticsDictionary = RaceManager.Instance.GetRaceStatistics();
        
        foreach(ulong id in idToRaceStatisticsDictionary.Keys)
        {
            GameObject instance;
            string placeText = "";
            if (idToRaceStatisticsDictionary[id].RaceFinished)
            {
                instance = Instantiate(statisticsPlayerCardPrefab, positionsList[idToRaceStatisticsDictionary[id].FinishedAtPlace - 1]);
                if (idToRaceStatisticsDictionary[id].FinishedAtPlace == 1) placeText = "1st";
                else if (idToRaceStatisticsDictionary[id].FinishedAtPlace == 2) placeText = "2nd";
                else if (idToRaceStatisticsDictionary[id].FinishedAtPlace == 3) placeText = "3rd";
                else placeText = $"{idToRaceStatisticsDictionary[id].FinishedAtPlace}th";
            }
            else
            {
                int index;
                for (index = positionTakenList.Count - 1; index > 0; index--)
                {
                    if (positionTakenList[index]) continue;
                    break;
                }
                instance = Instantiate(statisticsPlayerCardPrefab, positionsList[index]);
                positionTakenList[index] = true;
                placeText = "DNF";
            }

            instance.transform.Find("FrogVisual").Find("Body").GetComponent<Image>().color = LobbyManager.Instance.GetFrogColor(id);
            instance.transform.Find("Texts").Find("PositionText").GetComponent<TextMeshProUGUI>().text = placeText;
            instance.transform.Find("Texts").Find("NameText").GetComponent<TextMeshProUGUI>().text = LobbyManager.Instance.GetFrogName(id);
            if (placeText != "DNF")
            {
                instance.transform.Find("Texts").Find("TimeText").GetComponent<TextMeshProUGUI>().text = idToRaceStatisticsDictionary[id].RaceTime.ToString("0.0");
            }
            else
            {
                instance.transform.Find("Texts").Find("TimeText").gameObject.SetActive(false);
            }            
        }


        StartCoroutine(UpdateCountdownTextAndLaunchLobbyScene());




        exitInsteadButton.onClick.AddListener(() =>
        {
            if (!NetworkManager.Singleton.IsServer)
            {
                SessionManager.Instance.QuitJoinedSession();
                
                SceneManager.LoadScene("Main Menu Scene");
            }
            else
            {
                SessionManager.Instance.QuitCreatedSession();
                
                SceneManager.LoadScene("Main Menu Scene");
            }
        });
    }







    private IEnumerator UpdateCountdownTextAndLaunchLobbyScene()
    {
        while (countdown >= 0)
        {
            yield return new WaitForSeconds(.2f);
            countdown -= .2f;

            CountdownText.text = Mathf.CeilToInt(countdown).ToString();

            
        }
        if (NetworkManager.Singleton.IsServer)
        {
            LobbyManager.Instance.LaunchServerRejoiningLobbyProcessRpc();
            RaceManager.Instance.GetComponent<NetworkObject>().Despawn();
            SessionManager.Instance.CreatedSessionStartAcceptingNewPlayers();
            NetworkManager.Singleton.SceneManager.LoadScene("Lobby Scene", LoadSceneMode.Single);
        }
    }
}
