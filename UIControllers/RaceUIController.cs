using System;
using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RaceUIController : BaseUIController
{
    public event EventHandler OnJumpButtonPressed;
    public event EventHandler OnZoomButtonPressed;
    public event EventHandler OnSwitchSideButtonPressed;

    [SerializeField] private string goMessage = "Skip!";

    [SerializeField] private Sprite zoomInSprite;
    [SerializeField] private Sprite zoomOutSprite;

    [SerializeField] private Button zoomButton;

    [SerializeField] private TextMeshProUGUI raceTimerText;
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private TextMeshProUGUI raceOverText;

    [SerializeField] private Button exitButton;

    [SerializeField] private GameObject areYouSureGroup;
    [SerializeField] private Button yesIamSureButton;
    [SerializeField] private Button noCancelThatButton;


    private enum ZoomState
    {
        ZoomedIn,
        HalfZoom,
        ZoomedOut
    }

    private ZoomState zoomState = ZoomState.ZoomedIn;






    private bool raceTimerActive = false;
    private bool raceOver = false;





    private void Start()
    {
        exitButton.onClick.AddListener(() =>
        {
            exitButton.interactable = false;
            areYouSureGroup.SetActive(true);
        });

        yesIamSureButton.onClick.AddListener(() =>
        {
            if (NetworkManager.Singleton.IsServer)
            {
                SessionManager.Instance.QuitCreatedSession();
                SceneManager.LoadScene("Main Menu Scene");
            }
            else
            {
                RaceManager.Instance.ServerRemovePlayerRpc(NetworkManager.Singleton.LocalClientId);
                SessionManager.Instance.QuitJoinedSession();
                SceneManager.LoadScene("Main Menu Scene");
            }
        });

        noCancelThatButton.onClick.AddListener(() =>
        {
            areYouSureGroup.SetActive(false);
            exitButton.interactable = true;
        });
    }





    public void JumpPress()
    {
        OnJumpButtonPressed?.Invoke(this, EventArgs.Empty);
    }
    public void ZoomPress()
    {
        OnZoomButtonPressed?.Invoke(this, EventArgs.Empty);

        if (zoomState == ZoomState.ZoomedIn)
        {
            zoomState = ZoomState.HalfZoom;
        }
        else if (zoomState == ZoomState.HalfZoom)
        {
            zoomState = ZoomState.ZoomedOut;
            zoomButton.GetComponent<Image>().sprite = zoomInSprite;
        }
        else if (zoomState == ZoomState.ZoomedOut)
        {
            zoomState = ZoomState.ZoomedIn;
            zoomButton.GetComponent<Image>().sprite = zoomOutSprite;
        }
    }
    public void SwitchSidePress()
    {
        OnSwitchSideButtonPressed?.Invoke(this, EventArgs.Empty);
    }




    public void ShowCountdownToStart()
    {
        countdownText.gameObject.SetActive(true);
        StartCoroutine(UpdateCountdownToStartEvery(.2f));
    }

    public void ShowRaceTimer()
    {
        raceTimerText.gameObject.SetActive(true);
        raceTimerActive = true;
        StartCoroutine(UpdateRaceTimerEvery(.1f));
    }

    public void ShowCountdownToFinish()
    {
        countdownText.gameObject.SetActive(true);
        StartCoroutine(UpdateCountdownToFinishEvery(.1f));
    }


    public void ShowRaceOverMessage()
    {
        raceOver = true;
        raceTimerActive = false;

        raceOverText.gameObject.SetActive(true);
    }



    private IEnumerator UpdateCountdownToStartEvery(float seconds)
    {
        bool countdownOver = false;
        while (!countdownOver)
        {
            yield return new WaitForSeconds(seconds);
            float countdown = RaceManager.Instance.GetCountdownToStartValue();

            if (countdown <= 0f)
            {
                countdownText.text = goMessage;
                countdownOver = true;
                StartCoroutine(HideCountdownTextAfter(1f));
            }
            else
            {
                countdownText.text = Mathf.Ceil(countdown).ToString();
            }
        }
    }

    private IEnumerator HideCountdownTextAfter(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        countdownText.gameObject.SetActive(false);
    }


    private IEnumerator UpdateRaceTimerEvery(float seconds)
    {
        while (raceTimerActive)
        {
            yield return new WaitForSeconds(seconds);
            float timer = RaceManager.Instance.GetRaceTimerValue();

            raceTimerText.text = timer.ToString(".0");
        }
    }

    private IEnumerator UpdateCountdownToFinishEvery(float seconds)
    {
        while (!raceOver)
        {
            yield return new WaitForSeconds(seconds);
            float countdown = RaceManager.Instance.GetCountdownToFinishValue();

            if (countdown <= 0f)
            {
                countdownText.text = "DNF!";
                StartCoroutine(HideCountdownTextAfter(1f));
                raceOver = true;
            }
            else
            {
                countdownText.text = Mathf.Ceil(countdown).ToString();
            }
        }
    }
}
