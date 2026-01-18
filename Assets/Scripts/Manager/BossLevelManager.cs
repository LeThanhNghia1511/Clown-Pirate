using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BossLevelManager : MonoBehaviour
{
    [SerializeField] private GameObject _boss;
    [SerializeField] private bool _isBossDefeated = false;
    [SerializeField] private int _point;

    public static BossLevelManager instance;
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
            AudioManager.instance.PlayMusic("bossFight");
            AudioManager.instance.PlayBGSFX("sea");
        }
        UIManager.instance.UpdatePointText(_point);
    }

    private void Update()
    {
        if (_isBossDefeated == false)
        {
            CheckCondition();
        }
    }

    private void CheckCondition()
    {
        if (_boss == null )
        {
            _isBossDefeated = true;
            AudioManager.instance.StopAllMusic();
            Debug.Log("Boss Defeated!");
            StartCoroutine(EndGame());
        }
    }

    public void AddPoint(int value)
    {
        _point += value;
        UIManager.instance.UpdatePointText(_point);
    }

    IEnumerator EndGame()
    {
        yield return new WaitForSeconds(7f);
        SceneManager.LoadScene("EndScene");
    }
}
