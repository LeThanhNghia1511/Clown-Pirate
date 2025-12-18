using UnityEngine;
public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private float _maxHealth = 10f;
    [SerializeField] private float _currentHealth = 0f;
    [Header("Live")]
    [SerializeField] private int _maxLive = 2;
    [SerializeField] private int _currentLive = 0;
    // Animator for some effects
    private Animator _animator;

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
        _currentHealth = _maxHealth;
        _currentLive = _maxLive;
        UIManager.instance.UpdateLiveText(_currentLive);
        _animator.SetBool("isDead", false);
    }

    public void TakeDamage(float damage, Vector2 hitDirection)
    {
        if (_currentHealth > 0)
        {
            _currentHealth -= damage;
            if (_animator != null)
            {
                _animator.SetTrigger("Hit");
                //Knockback.instance.DoKnockback();
                UIManager.instance.UpdateHPBar(_currentHealth, _maxHealth);
            }

            //2.Kích hoạt Knockback
            PlayerController player = GetComponent<PlayerController>();
            if (player != null)
            {
                //player.ApplyKnockback(hitDirection); // Gọi hàm đẩy lùi trong Controller
            }

            CameraManager.instance.ShakeCamera(3f, 0.3f);
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
}