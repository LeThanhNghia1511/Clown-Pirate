using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    private void Start()
    {
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayMusic("mainMenuBGM");
        }
    }
}
