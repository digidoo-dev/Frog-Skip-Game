using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InitializationUIController : BaseUIController
{
    [SerializeField] private GameObject frogSkipLogo;

    [SerializeField] private GameObject errorMessageGroup;
    [SerializeField] private TextMeshProUGUI errorMessageText;

    [SerializeField] private GameObject backgroundOverlay;

    private void Start()
    {
        SessionManager.Instance.OnInitializeSuccess += SessionManager_OnInitializeSuccess;
        SessionManager.Instance.OnInitializeError += SessionManager_OnInitializeError;
    }

    private void SessionManager_OnInitializeError(object sender, System.EventArgs e)
    {
        string errorMessage = (e as EventArgsCollection.ErrorMessageArgs).ErrorMessage;
        errorMessageText.text = errorMessage;

        errorMessageGroup.SetActive(true);

    }

    private void SessionManager_OnInitializeSuccess(object sender, System.EventArgs e)
    {
        StartCoroutine(LoadMainMenuAfterAllAnimationsEnd());
    }




    private IEnumerator LoadMainMenuAfterAllAnimationsEnd()
    {
        while(frogSkipLogo.GetComponent<Animation>().isPlaying)
        {
            yield return null;
        }
        backgroundOverlay.GetComponent<Animation>().Play();
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("Main Menu Scene");
    }
}
