using System;
using System.Collections.Generic;
using UnityEngine;

public static class EventArgsCollection
{
    public class ErrorMessageArgs : EventArgs
    {
        public string ErrorMessage { get; private set; }

        public ErrorMessageArgs(string errorMessage) 
        { 
            ErrorMessage = errorMessage;
        }
    }

    public class IntegerArgs : EventArgs
    {
        public int Value { get; private set; }
        public IntegerArgs(int value) { Value = value; }

    }

    public class MainMenuStateChangeArgs : EventArgs
    {
        public ProgramStates.MainMenuStates CurrentState { get; private set; }
        public ProgramStates.MainMenuStates NewState { get; private set; }
        public string ErrorMsg { get; private set; }

        public MainMenuStateChangeArgs(ProgramStates.MainMenuStates currentState, ProgramStates.MainMenuStates newState, string error="")
        {
            CurrentState = currentState;
            NewState = newState;
            ErrorMsg = error;
        }
    }

    public class PlayOptionsStateChangeArgs : EventArgs
    {
        public ProgramStates.PlayOptionsStates CurrentState { get; private set; }
        public ProgramStates.PlayOptionsStates NewState { get; private set; }
        public string ErrorMsg { get; private set; }

        public PlayOptionsStateChangeArgs(ProgramStates.PlayOptionsStates currentState, ProgramStates.PlayOptionsStates newState, string error = "")
        {
            CurrentState = currentState;
            NewState = newState;
            ErrorMsg = error;
        }
    }

    public class LobbyPlayerInfoArgs : EventArgs
    {
        public Dictionary<ulong, LobbyPlayerInfo> ClientIdToInfoDictionary { get; private set; }

        public LobbyPlayerInfoArgs(Dictionary<ulong, LobbyPlayerInfo> clientIdToInfoDictionary)
        {
            ClientIdToInfoDictionary = clientIdToInfoDictionary;
        }
    }

    public class LobbyPlayerIdAndReadinessArgs : EventArgs
    {
        public ulong Id { get; private set; }
        public bool Readiness { get; private set; }

        public LobbyPlayerIdAndReadinessArgs(ulong id, bool readiness)
        {
            Id = id;
            Readiness = readiness;
        }
    }

    public class ChatMessageArgs : EventArgs
    {
        public string PlayerName { get; private set; }
        public string Message { get; private set; }

        public ChatMessageArgs(string playerName, string message)
        {
            PlayerName = playerName;
            Message = message;
        }
    }
}
