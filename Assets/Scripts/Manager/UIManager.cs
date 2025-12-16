using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Image _imgHealth;
    [SerializeField] private TextMeshProUGUI _liveText;
    [SerializeField] private Animator _UIAnimator;
    [SerializeField] private Canvas _canvas;
    private GameObject _pauseMenu;
    private GameObject _winMenu;
    private GameObject _loseMenu;
    private TextMeshProUGUI _scoreText;
    private TextMeshProUGUI _winScoreText;
    private TextMeshProUGUI _loseScoreText;

    // Singleton
    public static UIManager instance;
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

        _winMenu = FindChildByName(_canvas.transform, "WinMenu");
        _loseMenu = FindChildByName(_canvas.transform, "LoseMenu");
        _pauseMenu = FindChildByName(_canvas.transform, "PauseMenu");
    }

    private void Start()
    {
        Transform scoreTextTransform = _canvas.transform.Find("ScoreText");
        _scoreText = scoreTextTransform.GetComponent<TextMeshProUGUI>();
        // ⭐ 2. KIỂM TRA VÀ LẤY SCORE TEXT TỪ CÁC PANEL
        if (_winMenu != null)
        {
            Transform winScoreTransform = _winMenu.transform.Find("ScoreText");
            if (winScoreTransform != null)
            {
                _winScoreText = winScoreTransform.GetComponent<TextMeshProUGUI>();
            }
        }

        if (_loseMenu != null)
        {
            Transform loseScoreTransform = _loseMenu.transform.Find("ScoreText");
            if (loseScoreTransform != null)
            {
                _loseScoreText = loseScoreTransform.GetComponent<TextMeshProUGUI>();
            }
        }

        UpdateHPBar(1, 1);
        //HidePauseMenu();
        //HideLoseMenu();
        //HideWinMenu();
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("TestLevel");
    }

    public void UpdateHPBar(float currentHealth, float maxHealth)
    {
        _imgHealth.fillAmount = currentHealth / maxHealth;
    }

    public void StartMainMenuAnimation()
    {
        _UIAnimator.SetTrigger("MainMenu");
    }

    public void StartLevelMenuAnimation()
    {
        _UIAnimator.SetTrigger("LevelMenu");
    }

    public void ShowPauseMenu()
    {
        _pauseMenu.SetActive(true);
        _winMenu.SetActive(false);
        _loseMenu.SetActive(false);
    }

    public void HidePauseMenu()
    {
        _pauseMenu.SetActive(false);
    }

    public void UpdateLiveText(int currentLive)
    {
        _liveText.text = "x" + currentLive;
    }

    public void ShowWinMenu()
    {
        _winMenu.SetActive(true);
        _pauseMenu.SetActive(false);
        _loseMenu.SetActive(false);
    }

    public void HideWinMenu()
    {
        _winMenu?.SetActive(false);
    }

    public void ShowLoseMenu()
    {
        _loseMenu.SetActive(true);
        _winMenu.SetActive(false);
        _pauseMenu.SetActive(false);
    }

    public void HideLoseMenu()
    {
        _loseMenu.SetActive(false);
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void UpdatePointText(int point)
    {
        if (_scoreText == null) return;
        if (_winScoreText == null) return;
        if (_loseScoreText == null) return;
        _scoreText.text = "Score: " + point;
        _winScoreText.text = "Score: " + point;
        _loseScoreText.text = "Score: " + point;
    }

    private GameObject FindChildByName(Transform parent, string name)
    {
        Transform childTransform = parent.Find(name);
        if (childTransform == null)
        {
            Debug.Log("Error");
            return null;
        }
        return childTransform.gameObject;
    }
}
