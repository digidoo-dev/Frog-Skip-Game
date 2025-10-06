using UnityEngine;

public static class PlayerInfoHolder
{
    private const string FROG_NAME_PLAYER_PREFS_KEY = "frogName";
    private const string FROG_COLOR_RED_PLAYER_PREFS_KEY = "frogColorRed";
    private const string FROG_COLOR_GREEN_PLAYER_PREFS_KEY = "frogColorGreen";
    private const string FROG_COLOR_BLUE_PLAYER_PREFS_KEY = "frogColorBlue";

    public static string FrogName { get; private set; } = "Skipper";
    public static Color FrogColor { get; private set; } = Color.green;


    public static void LoadPlayerPrefs()
    {
        FrogName = PlayerPrefs.GetString(FROG_NAME_PLAYER_PREFS_KEY, "Skipper");
        float r = PlayerPrefs.GetFloat(FROG_COLOR_RED_PLAYER_PREFS_KEY, 0f);
        float g = PlayerPrefs.GetFloat(FROG_COLOR_GREEN_PLAYER_PREFS_KEY, 1f);
        float b = PlayerPrefs.GetFloat(FROG_COLOR_BLUE_PLAYER_PREFS_KEY, 0f);
        FrogColor = new Color(r, g, b);
    }

    public static void SavePlayerPrefs(string frogName, Color frogColor)
    {
        FrogName = frogName;
        FrogColor = frogColor;

        PlayerPrefs.SetString(FROG_NAME_PLAYER_PREFS_KEY, FrogName);
        PlayerPrefs.SetFloat(FROG_COLOR_RED_PLAYER_PREFS_KEY, FrogColor.r);
        PlayerPrefs.SetFloat(FROG_COLOR_GREEN_PLAYER_PREFS_KEY, FrogColor.g);
        PlayerPrefs.SetFloat(FROG_COLOR_BLUE_PLAYER_PREFS_KEY, FrogColor.b);

        PlayerPrefs.Save();
    }
}
