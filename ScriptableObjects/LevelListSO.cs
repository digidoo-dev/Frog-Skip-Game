using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelListSO", menuName = "Scriptable Objects/LevelListSO")]
public class LevelListSO : ScriptableObject
{
    public List<LevelSO> levelList;
}
