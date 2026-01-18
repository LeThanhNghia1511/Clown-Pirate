using UnityEngine;

public class PlayerEnergy : MonoBehaviour
{
    [Header("Energy")]
    [SerializeField] private float _maxEnergy = 100f;
    [SerializeField] private float _currentEnergy;

    public static PlayerEnergy instance;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(instance);
        }
        else
        {
            instance = this;
        }
        _currentEnergy = _maxEnergy;
    }

    private void Start()
    {
        UIManager.instance.UpdateEnergyBar(_currentEnergy, _maxEnergy);
    }

    public bool HaveEnergy() // Energy > 0
    {
        if (_currentEnergy > 0) return true;
        else return false;
    }

    public void LoseEnergy(float loseAmount)
    {
        _currentEnergy -= loseAmount;
        UIManager.instance.UpdateEnergyBar(_currentEnergy, _maxEnergy);
    }

    public void RegenEnergy(float regenAmount)
    {
        _currentEnergy += regenAmount;
        if (_currentEnergy > _maxEnergy)
            _currentEnergy = _maxEnergy;

        UIManager.instance.UpdateEnergyBar(_currentEnergy, _maxEnergy);
    }
}
