using UnityEngine;

public class MainMenuManagr : MonoBehaviour
{
    private void Start()
    {
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayMainMenuBGM();
        }
    }
}
