using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HowToUIController : BaseUIController
{
    [SerializeField] private List<GameObject> howToPanelsList;

    [SerializeField] private Button previousPanelButton;
    [SerializeField] private Button nextPanelButton;


    private int currentlyShownPanelIndex = 0;


    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("Main Menu Scene");
    }

    public void ShowNextPanel()
    {
        if (currentlyShownPanelIndex == howToPanelsList.Count - 1) return;

        previousPanelButton.gameObject.SetActive(true);

        howToPanelsList[currentlyShownPanelIndex].SetActive(false);

        currentlyShownPanelIndex++;

        howToPanelsList[currentlyShownPanelIndex].SetActive(true);
        
        if (currentlyShownPanelIndex == howToPanelsList.Count - 1)
        {
            nextPanelButton.gameObject.SetActive(false);
        }
    }

    public void ShowPreviousPanel()
    {
        if (currentlyShownPanelIndex == 0) return;

        nextPanelButton.gameObject.SetActive(true);

        howToPanelsList[currentlyShownPanelIndex].SetActive(false);

        currentlyShownPanelIndex--;

        howToPanelsList[currentlyShownPanelIndex].SetActive(true);

        if (currentlyShownPanelIndex == 0)
        {
            previousPanelButton.gameObject.SetActive(false);
        }
    }
}
