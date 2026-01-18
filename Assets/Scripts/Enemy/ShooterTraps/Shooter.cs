using Unity.VisualScripting;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] protected float _maxHealth = 3f;
    [SerializeField] protected float _currentHealth = 0f;

    [Header("Live")]
    [SerializeField] protected int _maxLive = 1;
    [SerializeField] protected int _currentLive = 0;

    [Header("AI Setting")]
    [SerializeField] protected float _shootRange = 10f;
    [SerializeField] protected GameObject _projectilePrefab;
    [SerializeField] protected GameObject _debrisObject;
    [SerializeField] protected Transform _projectilePosition;
    [SerializeField] protected float _shootCooldown = 3f;
    private float _shootCounter;

    [Header("Score")]
    [SerializeField] protected int _pointValue = 20;

    [Header("Layers")]
    [SerializeField] protected LayerMask _playerLayer;

    protected Animator _animator;
    protected Rigidbody2D _rb;
    private Knockback _knockback;
    protected Vector3 _startPosition;
    protected bool _isFacingRight = false;
    protected bool _isAttacking = false;
    protected bool _isDead = false;


    private void Awake()
    {
        _animator = this.GetComponent<Animator>();
        _rb = this.GetComponent<Rigidbody2D>();
        _knockback = this.GetComponent<Knockback>();
        _currentHealth = _maxHealth;
        _currentLive = _maxLive;
        _isFacingRight = _rb.transform.localScale.x < 0 ? true : false;
        _isDead = false;
        _shootCounter = _shootCooldown;
        if (_animator != null)
        {

            _animator.SetBool(AnimationStrings.isDead, _isDead);
        }
    }

    public virtual void Update()
    {
        if (_knockback.IsBeingKnocked) return;
        CheckForPlayer();
        _shootCounter -= Time.deltaTime;
    }

    private void Flip()
    {
        _rb.transform.localScale = new Vector3(_rb.transform.localScale.x * -1, 1f, 1f);
        _isFacingRight = !_isFacingRight;
    }

    #region Attack
    public virtual void CheckForPlayer()
    {
        Vector2 castStartPos = this.transform.position;
        Vector2 castDir = _isFacingRight ? Vector2.right : Vector2.left;
        // Shoot
        RaycastHit2D _shootPlayer = Physics2D.Raycast(
                                        castStartPos, // Start position of raycast
                                        castDir, // Raycast direction
                                        10f, // Distance fom enemy to player
                                        _playerLayer); // Layer

        if (_shootPlayer.collider != null)
        {
            Shoot();
        }

        // Show raycast
        //Color rayColor = _shootPlayer.collider != null ? Color.red : Color.green; // The color turn red when hit the wall
        //Debug.DrawRay(
        //    castStartPos,
        //    castDir * 1f,
        //    rayColor,
        //    Time.fixedDeltaTime
        //);
    }

    protected void Shoot()
    {
        if (_shootCounter > 0) return;
        _animator.SetTrigger(AnimationStrings.shoot);
        _shootCounter = _shootCooldown;
    }

    public virtual void AnimationEvent_Shoot()
    {
        GameObject bullet = Instantiate(_projectilePrefab, _projectilePosition.position, Quaternion.identity);
        Vector2 shootDir = _isFacingRight ? Vector2.right : Vector2.left;
        bullet.GetComponent<Bullet>().Initialize(shootDir);
    }
    #endregion

    #region Take Damage and Death
    public void TakeDamage(float damage, float knockbackForce, Vector2 damageSourcePos)
    {
        if (_isDead) return;
        _currentHealth -= damage;
        _animator.SetTrigger(AnimationStrings.hit);
        PlayHitSFX();
        _knockback.GetKnocked(knockbackForce, damageSourcePos);
        if (_currentHealth <= 0)
        {
            HandleDeath();
        }
    }

    public virtual void PlayHitSFX()
    {
        AudioManager.instance.PlaySFX("hit");
    }

    private void HandleDeath()
    {
        _currentLive--;
        if (_currentLive <= 0)
        {
            _isDead = true;
            _animator.SetBool(AnimationStrings.isDead, true);
            _animator.SetTrigger(AnimationStrings.hit);
        }
    }

    public void Animation_Kill()
    {
        if (LevelManager.instance != null)
            LevelManager.instance.AddPoint(_pointValue);
        if (BossLevelManager.instance != null)
            BossLevelManager.instance.AddPoint(_pointValue);
        PlayDeathSFX();
        GameObject debris = Instantiate(_debrisObject, transform.position, Quaternion.identity);
        Destroy(this.gameObject);
    }

    public virtual void PlayDeathSFX()
    {
        AudioManager.instance.PlaySFX("barrelBreak");
    }
    #endregion
}
