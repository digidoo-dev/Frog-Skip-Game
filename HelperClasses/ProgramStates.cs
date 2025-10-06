using UnityEngine;

public static class ProgramStates
{

    public enum MainMenuStates
    {
        IntroAnimation,
        MainButtons,
        EditFrog,
        OutroAnimation
    }

    public enum PlayOptionsStates
    {
        Lobbies,
        CreateNewLobby,
        JoinLobbyByCode,
        Connecting,
        Error
    }
}
