using TMPro;
using UnityEngine;
public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private float _maxHealth = 10f;
    [SerializeField] private float _currentHealth = 0f;
    private bool _isDead;
    [Header("Live")]
    [SerializeField] private int _maxLive = 2;
    [SerializeField] private int _currentLive = 0;
    [Header("Drowning")]
    [SerializeField] private float _drownDamage = 10f;
    [SerializeField] private float _drownFrequency = 1f;
    private float _drownCounter = 0f;
    // Animator for some effects
    private Animator _animator;
    private Knockback _knockback;

    // Singleton
    public static PlayerHealth instance;
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

        _animator = this.GetComponent<Animator>();
        _knockback = this.GetComponent<Knockback>();
        _currentHealth = _maxHealth;
        _currentLive = _maxLive;
        _animator.SetBool(AnimationStrings.isDead, false);
        _isDead = false;
    }

    private void Start()
    {
        UIManager.instance.UpdateHPBar(_currentHealth, _maxHealth);
        UIManager.instance.UpdateLiveText(_currentLive);
    }

    private void Update()
    {
        _drownCounter += Time.deltaTime;
    }

    public void TakeDamage(float damage, float knockbackForce, Vector2 damageSourcePos)
    {
        if (_isDead) return;
        if (_currentHealth > 0)
        {
            _currentHealth -= damage;
            AudioManager.instance.PlaySFX("hit");
            if (_animator != null)
            {
                _animator.SetTrigger(AnimationStrings.hit);
                UIManager.instance.UpdateHPBar(_currentHealth, _maxHealth);
            }
            _knockback.GetKnocked(knockbackForce, damageSourcePos);
            CameraManager.instance.ShakeCamera(0.5f, 0.3f);
        }
        if (_currentHealth <= 0)
        {
            HandleDeath();
        }
    }

    private void HandleDeath()
    {
        _currentLive--;
        if (_currentLive >= 0)
        {
            UIManager.instance.UpdateLiveText(_currentLive);
        }
        if (_currentLive > 0)
        {
            float stillDamage = _currentHealth;
            _currentHealth = _maxHealth;
            _currentHealth += stillDamage;
            UIManager.instance.UpdateHPBar(_currentHealth, _maxHealth);
        }
        else
        {
            _isDead = true;
            PlayerController.instance.LockControlsOnDeath();
            _animator.SetBool(AnimationStrings.isDead, true);
            _animator.SetTrigger(AnimationStrings.hit);
            DialogueEvent.Trigger(gameObject, "Dead");
        }
    }

    public void Heal(float healAmount)
    {
        _currentHealth += healAmount;
        DialogueEvent.Trigger(gameObject, "Exclamation");
        if (_currentHealth > _maxHealth)
        {
            _currentHealth = _maxHealth;
        }
        UIManager.instance.UpdateHPBar(_currentHealth, _maxHealth);
    }

    public void Drown()
    {
        if (_drownCounter >= _drownFrequency)
        {
            _drownCounter = 0f;
            TakeDamage(_drownDamage, 5f,transform.position);
        }
    }
}