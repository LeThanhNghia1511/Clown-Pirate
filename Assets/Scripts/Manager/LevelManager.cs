using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private int _point = 0;
    [SerializeField] private int _requiredPoint = 100;
    // Singleton
    public static LevelManager instance;
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
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayMusic("gameBGM");
            AudioManager.instance.PlayBGSFX("sea");
        }
        UIManager.instance.UpdatePointText(_point);
    }

    public void AddPoint(int value)
    {
        _point += value;
        UIManager.instance.UpdatePointText(_point);
        if (_point >= _requiredPoint)
        {
            GameManager.instance.WinGame();
            PassLevel();
        }
    }

    private void PassLevel()
    {
        int currentLevelIndex = SceneManager.GetActiveScene().buildIndex;
        int reachedLevel = PlayerPrefs.GetInt("ReachedLevel", 1);

        if (currentLevelIndex >= reachedLevel)
        {
            if (currentLevelIndex == 6)
            {
                bool hasBossItem = PlayerPrefs.GetInt("UnlockedBossLevel", 0) == 1;

                if (hasBossItem)
                {
                    PlayerPrefs.SetInt("ReachedLevel", 7);
                    Debug.Log("Unlock Boss Level");
                }
                else
                {
                    Debug.Log("Still not meet condition to unlock boss level");
                }
            }
            else
            {
                PlayerPrefs.SetInt("ReachedLevel", reachedLevel + 1);
                Debug.Log("Unlock next level: " + (reachedLevel + 1));
            }

            PlayerPrefs.Save();
        }
    }
}
