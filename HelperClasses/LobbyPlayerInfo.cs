using UnityEngine;

public class LobbyPlayerInfo
{
    public string FrogName { get; private set; }
    public Color FrogColor { get; private set; }
    public bool Readiness { get; private set; }

    public LobbyPlayerInfo(string name, Color color, bool readiness=false)
    {
        FrogName = name;
        FrogColor = color;
        Readiness = readiness;
    }

    public void SwitchReadiness() { Readiness = !Readiness; }
    public void SetReadiness(bool readiness) { Readiness = readiness; }
}
