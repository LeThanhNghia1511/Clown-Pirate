using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Data.Common;
using Unity.VisualScripting;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Image _imgHealth;
    [SerializeField] private Color[] _liveColors;
    [SerializeField] private Image _imgEnergy;
    [SerializeField] private Image _imgKey;
    [SerializeField] private TextMeshProUGUI _liveText;
    [SerializeField] private Animator _UIAnimator;
    [SerializeField] private Canvas _canvas;
    private GameObject _pauseMenu;
    private GameObject _winMenu;
    private GameObject _loseMenu;
    private TextMeshProUGUI _scoreText;
    private TextMeshProUGUI _winScoreText;
    private TextMeshProUGUI _loseScoreText;
    private Coroutine _hpCoroutine;
    private Coroutine _bossHpCoroutine;
    private Coroutine _energyCoroutine;
    [Header("Boss UI")]
    [SerializeField] private GameObject _bossHPBar;
    [SerializeField] private Image _imgBossHP;

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
        _liveColors = new Color[3] { Color.gray, Color.green, Color.red};
        _imgKey.gameObject.SetActive(false);

        Transform scoreTextTransform = _canvas.transform.Find("ScoreText");
        _scoreText = scoreTextTransform.GetComponent<TextMeshProUGUI>();
        if (_winMenu != null)
        {
            Transform winScoreTransform = _winMenu.transform.Find("Big/ScoreText");
            if (winScoreTransform != null)
            {
                _winScoreText = winScoreTransform.GetComponent<TextMeshProUGUI>();
            }
        }

        if (_loseMenu != null)
        {
            Transform loseScoreTransform = _loseMenu.transform.Find("Big/ScoreText");
            if (loseScoreTransform != null)
            {
                _loseScoreText = loseScoreTransform.GetComponent<TextMeshProUGUI>();
            }
        }
    }

    private void Start()
    {
        UpdatePointText(0);
        // Unactive UI menu
        _pauseMenu.SetActive(false);
        _winMenu.SetActive(false);
        _loseMenu.SetActive(false);
    }

    // Stats
    public void ActiveKeyUI()
    {
        _imgKey.gameObject.SetActive(true);
    }

    public void UpdateLiveText(int currentLive)
    {
        _liveText.text = "x" + currentLive;
    }

    public void UpdatePointText(int point)
    {
        if (_scoreText != null)
        {
            _scoreText.text = point.ToString();
        }
        else
        {
            Debug.LogWarning("Cant find the score text!");
        }

        if (_winScoreText != null)
        {
            _winScoreText.text = "Score: " + point;
        }

        if (_loseScoreText != null)
        {
            _loseScoreText.text = "Score: " + point;
        }
    }

    // HP
    public void UpdateHPBar(float currentHealth, float maxHealth)
    {
        float targetFill = currentHealth / maxHealth;
        if (_hpCoroutine != null) StopCoroutine(_hpCoroutine);
        _hpCoroutine = StartCoroutine(SmoothBarUpdate(_imgHealth, targetFill));
    }

    public void UpdateHealthBarColor(int currentLive)
    {
        if (_imgBossHP == null || _liveColors.Length == 0) return;
        _imgBossHP.color = _liveColors[currentLive - 1];
    }

    // Energy
    public void UpdateEnergyBar(float currentEnergy, float maxEnergy)
    {
        float targetFill = currentEnergy / maxEnergy;
        if (_energyCoroutine != null) StopCoroutine(_energyCoroutine);
        _energyCoroutine = StartCoroutine(SmoothBarUpdate(_imgEnergy, targetFill));
    }

    IEnumerator SmoothBarUpdate(Image image, float target)
    {
        while (Mathf.Abs(image.fillAmount - target) > 0.001f)
        {
            image.fillAmount = Mathf.Lerp(image.fillAmount, target, 5f * Time.deltaTime);
            yield return null;
        }
        image.fillAmount = target;
    }

    // Boss UI
    public void ActivateBossUI(float maxHealth)
    {
        if (_bossHPBar != null)
        {
            _bossHPBar.SetActive(true);
        }
    }

    public void UpdateBossHP(float currentHealth, float maxHealth)
    {
        if (_imgBossHP != null)
        {
            float targetFill = currentHealth / maxHealth;
            if (_bossHpCoroutine != null) StopCoroutine(_bossHpCoroutine);
            _bossHpCoroutine = StartCoroutine(SmoothBarUpdate(_imgBossHP, targetFill));
        }
    }

    // Menu UI
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

    // Helper
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
