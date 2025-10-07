using UnityEngine;

[CreateAssetMenu(fileName = "LevelSO", menuName = "Scriptable Objects/LevelSO")]
public class LevelSO : ScriptableObject
{
    public string levelName;
    public Sprite levelThumbnail;
    public string levelSceneName;
}
