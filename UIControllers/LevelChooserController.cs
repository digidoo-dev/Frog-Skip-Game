using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LevelChooserController : MonoBehaviour
{
    [SerializeField] private Image levelThumbnail;
    [SerializeField] private TextMeshProUGUI levelNameText;
    [SerializeField] private Button chooseLeftButton;
    [SerializeField] private Button chooseRightButton;



    private List<LevelSO> allLevelsList = new List<LevelSO>();
    private int currentlySelectedLevelIndex = 0;


    private void Awake()
    {
        allLevelsList = Resources.Load<LevelListSO>(typeof(LevelListSO).Name).levelList;
    }

    private void Start()
    {
        SetToLevel(currentlySelectedLevelIndex);

        if (!NetworkManager.Singleton.IsServer) return;

        chooseLeftButton.onClick.AddListener(() =>
        {
            currentlySelectedLevelIndex -= 1;
            if (currentlySelectedLevelIndex < 0) currentlySelectedLevelIndex = allLevelsList.Count - 1;
            Debug.Log("chooseLeftButton na serwerze index teraz = " + currentlySelectedLevelIndex);
            LobbyManager.Instance.ChangeSelectedLevelRpc(currentlySelectedLevelIndex);
        });

        chooseRightButton.onClick.AddListener(() =>
        {
            currentlySelectedLevelIndex += 1;
            if (currentlySelectedLevelIndex == allLevelsList.Count) currentlySelectedLevelIndex = 0;
            Debug.Log("chooseRightButton na serwerze index teraz = " + currentlySelectedLevelIndex);
            LobbyManager.Instance.ChangeSelectedLevelRpc(currentlySelectedLevelIndex);
        });
    }

    public void HideSelectionButtons()
    {
        chooseLeftButton.gameObject.SetActive(false);
        chooseRightButton.gameObject.SetActive(false);
    }

    public void SetToLevel(int index)
    {
        Debug.Log("LevelChooserController SetToLevel index=" + index.ToString());
        if (index >= allLevelsList.Count) return;
        LevelSO level = allLevelsList[index];

        levelThumbnail.sprite = level.levelThumbnail;
        levelNameText.text = level.levelName;

        currentlySelectedLevelIndex = index;
    }

}
