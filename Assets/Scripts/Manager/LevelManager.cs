using UnityEngine;

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
            AudioManager.instance.PlayGameBGM();
        }
    }

    public void AddPoint(int value)
    {
        _point += value;
        UIManager.instance.UpdatePointText(_point);
        if (_point >= _requiredPoint)
        {
            GameManager.instance.WinGame();
        }
    }
}
