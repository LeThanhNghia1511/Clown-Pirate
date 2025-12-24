using UnityEngine;
public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private float _maxHealth = 10f;
    [SerializeField] private float _currentHealth = 0f;
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
    }

    private void Start()
    {
        _animator = this.GetComponent<Animator>();
        _knockback = this.GetComponent<Knockback>();
        _currentHealth = _maxHealth;
        _currentLive = _maxLive;
        UIManager.instance.UpdateLiveText(_currentLive);
        _animator.SetBool("isDead", false);
    }

    private void Update()
    {
        _drownCounter += Time.deltaTime;
    }

    public void TakeDamage(float damage, float knockbackForce, Vector2 damageSourcePos)
    {
        if (_currentHealth > 0)
        {
            _currentHealth -= damage;
            AudioManager.instance.PlayHitSFX();
            if (_animator != null)
            {
                _animator.SetTrigger("Hit");
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
            _currentHealth = _maxHealth;
            UIManager.instance.UpdateHPBar(_currentHealth, _maxHealth);
        }
        else
        {
            PlayerController.instance.LockControlsOnDeath();
            _animator.SetBool("isDead", true);
            _animator.SetTrigger("DeadHit");
        }
    }

    public void Animation_Kill()
    {
        Destroy(this.gameObject);
        GameManager.instance.LoseGame();
    }

    public void Heal(float healAmount)
    {
        _currentHealth += healAmount;
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