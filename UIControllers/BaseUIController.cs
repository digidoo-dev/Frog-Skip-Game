using UnityEngine;

public abstract class BaseUIController : MonoBehaviour
{
    protected void SwitchLayout(GameObject fromGroup, GameObject toGroup)
    {
        fromGroup?.SetActive(false);
        toGroup?.SetActive(true);
    }
}
