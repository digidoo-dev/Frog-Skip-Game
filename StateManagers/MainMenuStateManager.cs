using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuStateManager : BaseStateManager
{
    public event EventHandler OnStateChanged;


    private ProgramStates.MainMenuStates state = ProgramStates.MainMenuStates.IntroAnimation;


    private void Start()
    {
        StartCoroutine(SwitchToMainButtonsAfterIntroAnimation(2f));
    }


    public void SwitchToMainButtons()
    {
        var currentState = state;
        var newState = ProgramStates.MainMenuStates.MainButtons;
        OnStateChanged?.Invoke(this, new EventArgsCollection.MainMenuStateChangeArgs(currentState, newState));

        state = newState;
    }
    public void SwitchToEditFrog()
    {
        var currentState = state;
        var newState = ProgramStates.MainMenuStates.EditFrog;
        OnStateChanged?.Invoke(this, new EventArgsCollection.MainMenuStateChangeArgs(currentState, newState));

        state = newState;
    }

    

    public void GoToPlayOptionsScene()
    {
        var currentState = state;
        var newState = ProgramStates.MainMenuStates.OutroAnimation;
        OnStateChanged?.Invoke(this, new EventArgsCollection.MainMenuStateChangeArgs(currentState, newState));

        StartCoroutine(LoadPlayOptionsSceneAfter(2f));
    }



    public void OpenHelpScene()
    {
        SceneManager.LoadScene("HowTo Scene");
    }




    private IEnumerator SwitchToMainButtonsAfterIntroAnimation(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        SwitchToMainButtons();
    }


    private IEnumerator LoadPlayOptionsSceneAfter(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        SceneManager.LoadScene("Play Options Scene");
    }
}
