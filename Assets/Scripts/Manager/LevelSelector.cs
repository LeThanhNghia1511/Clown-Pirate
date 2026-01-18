using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelSelector : MonoBehaviour
{
    [Header("Levels list")]
    [SerializeField] private Button[] _levelButtons;

    void Start()
    {
        int reachedLevel = PlayerPrefs.GetInt("ReachedLevel", 1);

        bool hasBossItem = PlayerPrefs.GetInt("UnlockedBossLevel", 0) == 1;

        for (int i = 0; i < _levelButtons.Length; i++)
        {
            int levelNum = i + 1; 

            if (levelNum <= reachedLevel)
            {
                if (levelNum == 7)
                {
                    if (hasBossItem)
                    {
                        UnlockButton(_levelButtons[i]);
                    }
                    else
                    {
                        LockButton(_levelButtons[i]);
                    }
                }
                else
                {
                    UnlockButton(_levelButtons[i]);
                }
            }
            else
            {
                LockButton(_levelButtons[i]);
            }
        }
    }

    private void UnlockButton(Button btn)
    {
        btn.interactable = true;
        btn.image.color = Color.white; 
    }

    private void LockButton(Button btn)
    {
        btn.interactable = false;
        btn.image.color = Color.gray;
    }

    public void OpenLevel(int levelIndex)
    {
        SceneManager.LoadScene(levelIndex);
    }
}