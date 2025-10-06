using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayOptionsStateManager : BaseStateManager
{
    public event EventHandler OnStateChanged;

    private ProgramStates.PlayOptionsStates state = ProgramStates.PlayOptionsStates.Lobbies;




    public void SwitchToLobbies()
    {
        var currentState = state;
        var newState = ProgramStates.PlayOptionsStates.Lobbies;
        OnStateChanged?.Invoke(this, new EventArgsCollection.PlayOptionsStateChangeArgs(currentState, newState));

        state = newState;
    }

    public void SwitchToCreateNewLobby()
    {
        var currentState = state;
        var newState = ProgramStates.PlayOptionsStates.CreateNewLobby;
        OnStateChanged?.Invoke(this, new EventArgsCollection.PlayOptionsStateChangeArgs(currentState, newState));

        state = newState;
    }

    public void SwitchToJoinLobbyByCode()
    {
        var currentState = state;
        var newState = ProgramStates.PlayOptionsStates.JoinLobbyByCode;
        OnStateChanged?.Invoke(this, new EventArgsCollection.PlayOptionsStateChangeArgs(currentState, newState));

        state = newState;
    }

    public void SwitchToConnecting()
    {
        var currentState = state;
        var newState = ProgramStates.PlayOptionsStates.Connecting;
        OnStateChanged?.Invoke(this, new EventArgsCollection.PlayOptionsStateChangeArgs(currentState, newState));

        state = newState;
    }

    public void SwitchToError(string errorMessage)
    {
        var currentState = state;
        var newState = ProgramStates.PlayOptionsStates.Error;
        OnStateChanged?.Invoke(this, new EventArgsCollection.PlayOptionsStateChangeArgs(currentState, newState, errorMessage));

        state = newState;
    }


    public void GoBackToMainMenu()
    {
        SceneManager.LoadScene("Main Menu Scene");
    }
}
