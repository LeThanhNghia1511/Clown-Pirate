using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private float _maxHealth = 10f;
    [SerializeField] private float _currentHealth = 0f;
    [Header("Live")]
    [SerializeField] private int _maxLive = 2;
    [SerializeField] private int _currentLive = 0;
    // Animator for some effects
    private Animator _animator;

    public static Health instance;
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
                UIManager.instance.UpdateHPBar(_currentHealth, _maxHealth);
            }

            //2.Kích hoạt Knockback
            PlayerController player = GetComponent<PlayerController>();
            if (player != null)
            {
                player.ApplyKnockback(hitDirection); // Gọi hàm đẩy lùi trong Controller
            }

            //3.Kích hoạt Camera Shake
            CameraManager.instance.ShakeCamera(10f, 10f);
        }
        if (_currentHealth <= 0)
        {
            DeathHandle();
        }
    }

    private void DeathHandle()
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
            GameManager.instance.LoseGame();
        }
    }

    private void Kill()
    {
        Destroy(this.gameObject);
    }
}