using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public bool isPausedMenuShowed;
    // Singleton
    public static GameManager instance;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }
    private void Start()
    {
        isPausedMenuShowed = false;
        Time.timeScale = 1.0f;
    }

    private void Update()
    {
        bool isMainMenuScene = SceneManager.GetActiveScene().name == "MainMenu";
        if (!isMainMenuScene)
        {
            if (Input.GetKeyDown(KeyCode.Escape) && isPausedMenuShowed)
            {
                Unpause();
            }
            else if (Input.GetKeyDown(KeyCode.Escape) && !isPausedMenuShowed)
            {
                Pause();
            }
        }
        // Restart - Co the Restart bat cu luc nao
        if (Input.GetKeyDown(KeyCode.R))
        {
            Restart();
        }
    }

    public void Pause()
    {
        Time.timeScale = 0f;
        UIManager.instance.ShowPauseMenu();
        isPausedMenuShowed = true;
    }

    public void Unpause()
    {
        Time.timeScale = 1f;
        UIManager.instance.HidePauseMenu();
        isPausedMenuShowed = false;
    }

    public void WinGame()
    {
        Time.timeScale = 0f;
        AudioManager.instance.StopGameBGM();
        AudioManager.instance.PlayGameWinMusic();
        UIManager.instance.ShowWinMenu();
    }

    public void LoseGame()
    {
        Time.timeScale = 0f;
        AudioManager.instance.StopGameBGM();
        AudioManager.instance.PlayGameOverMusic();
        UIManager.instance.ShowLoseMenu();
    }

    public void Restart()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }
    
    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void LoadLevel(string levelName)
    {
        if (levelName != null && levelName != "")
            SceneManager.LoadScene(levelName);
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit Game");
    }
}
