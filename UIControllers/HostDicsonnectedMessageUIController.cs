using UnityEngine;
using UnityEngine.SceneManagement;

public class HostDicsonnectedMessageUIController : BaseUIController
{
    public void ToMainMenu()
    {
        SceneManager.LoadScene("Main Menu Scene");
    }
}
