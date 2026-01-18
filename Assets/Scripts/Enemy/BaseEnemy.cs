using UnityEngine;

public class BaseEnemy : MonoBehaviour
{
    [Header("Health & Lives")]
    [SerializeField] protected float _maxHealth = 10f;
    protected float _currentHealth;
    protected bool _isDead = false;
    [SerializeField] private float _invincibilityDuration = 0.5f;
    private float _invincibilityTimer;
    [SerializeField] private int _maxLive = 1;
    protected int _currentLive;

    [Header("AI Patrol & Chase")]
    [SerializeField] private float _moveSpeed = 3f;
    [SerializeField] private float _chaseSpeed = 5f;
    [SerializeField] private float _moveRange = 10f;
    [SerializeField] private float _detectionRange = 7f;
    protected bool _isPlayerDetected = false;
    private Vector3 _startPosition;
    private bool _isFacingRight = false;

    [Header("Jump & Physics")]
    [SerializeField] private float _jumpForce = 20f;
    [SerializeField] private float _wallCheckDistance = 0.8f;
    [SerializeField] private LayerMask _wallLayer;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private Vector2 _boxSize = new Vector2(0.6f, 0.1f);
    private bool _isGrounded;

    [Header("Combat")]
    [SerializeField] protected Transform _playerTransform;
    [SerializeField] private float _attackCooldown = 2f;
    [SerializeField] private float _attackRange = 1.8f;
    protected float _attackCounter = 0f;

    [Header("Score")]
    [SerializeField] protected int _pointValue = 100;

    protected Animator _animator;
    protected Rigidbody2D _rb;
    private Knockback _knockback;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody2D>();
        _knockback = GetComponent<Knockback>();

        _startPosition = transform.position;
        _currentHealth = _maxHealth;
        _currentLive = _maxLive;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) _playerTransform = player.transform;
    }

    public void Update()
    {
        if (_isDead || (_knockback != null && _knockback.IsBeingKnocked)) return;

        if (_invincibilityTimer > 0) _invincibilityTimer -= Time.deltaTime;
        if (_attackCounter > 0) _attackCounter -= Time.deltaTime;

        _isGrounded = CheckGround();
        HandleAI();
        CheckFalling();
    }

    private void HandleAI()
    {
        if (_isDead) return;
        if (_playerTransform == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, _playerTransform.position);

        if (distanceToPlayer <= _attackRange)
        {
            AttackLogic();
        }
        else if (distanceToPlayer <= _detectionRange)
        {
            ChaseLogic();
        }
        else
        {
            PatrolLogic();
        }

        if (IsMoving())
        {
            CheckForJump();
        }
    }

    #region AI States
    private void PatrolLogic()
    {
        _isPlayerDetected = false;
        float leftBoundary = _startPosition.x - (_moveRange / 2f);
        float rightBoundary = _startPosition.x + (_moveRange / 2f);

        if (_isFacingRight && transform.position.x >= rightBoundary) Flip();
        else if (!_isFacingRight && transform.position.x <= leftBoundary) Flip();

        ApplyMovement(_moveSpeed);
    }

    private void ChaseLogic()
    {
        _isPlayerDetected = true;
        float direction = _playerTransform.position.x - transform.position.x;

        if (direction > 0 && !_isFacingRight) Flip();
        else if (direction < 0 && _isFacingRight) Flip();

        ApplyMovement(_chaseSpeed);
    }

    private void ApplyMovement(float speed)
    {
        float velocityX = _isFacingRight ? speed : -speed;
        _rb.linearVelocity = new Vector2(velocityX, _rb.linearVelocity.y);
        _animator.SetBool(AnimationStrings.isRunning, true);
    }

    public virtual void AttackLogic()
    {
        // Dừng lại để đánh
        _rb.linearVelocity = new Vector2(0, _rb.linearVelocity.y);
        _animator.SetBool(AnimationStrings.isRunning, false);

        if (_attackCounter <= 0f)
        {
            PlayAttackSFX();
            _animator.SetTrigger(AnimationStrings.attack);
            _attackCounter = _attackCooldown;
        }
    }

    #endregion

    #region Movement Helpers
    private void CheckForJump()
    {
        Vector2 direction = _isFacingRight ? Vector2.right : Vector2.left;
        RaycastHit2D hitWall = Physics2D.Raycast(transform.position, direction, _wallCheckDistance, _wallLayer);
        if (hitWall.collider != null && _isGrounded)
        {
            Vector2 force = new Vector2(0, _jumpForce);
            _rb.AddForce(force, ForceMode2D.Impulse);
            _animator.SetTrigger(AnimationStrings.jump);
        }
    }

    private void CheckFalling()
    {
        if (_rb.linearVelocity.y < -0.1f)
        {
            _animator.SetTrigger(AnimationStrings.fall);
        }
    }

    private bool CheckGround()
    {
        Vector2 rayOrigin = (Vector2)transform.position + Vector2.down * 0.5f;
        bool grounded = Physics2D.BoxCast(rayOrigin, _boxSize, 0f, Vector2.down, 0.2f, _groundLayer);
        _animator.SetBool(AnimationStrings.isGrounded, grounded);
        return grounded;
    }

    private bool IsMoving()
    {
        return Mathf.Abs(_rb.linearVelocity.x) > 0.1f;
    }

    private void Flip()
    {
        _isFacingRight = !_isFacingRight;
        Vector3 localScale = transform.localScale;
        float absoluteX = Mathf.Abs(localScale.x);
        localScale.x = _isFacingRight ? -absoluteX : absoluteX;
        transform.localScale = localScale;
    }
    #endregion

    #region Damage Logic
    public virtual void TakeDamage(float damage, float knockbackForce, Vector2 damageSourcePos)
    {
        if (_isDead || _invincibilityTimer > 0) return;

        _currentHealth -= damage;
        _attackCounter = _attackCooldown;
        AudioManager.instance.PlaySFX("hit");
        _invincibilityTimer = _invincibilityDuration;
        _animator.SetTrigger(AnimationStrings.hit);

        if (_knockback != null) _knockback.GetKnocked(knockbackForce, damageSourcePos);
        if (_currentHealth <= 0) HandleDeath();
    }

    private void HandleDeath()
    {
        _currentLive--;
        PlayDeathSFX();
        _isDead = true;
        if (_currentLive <= 0)
        {
            _rb.linearVelocity = Vector2.zero;
            if (LevelManager.instance != null)
            {
                LevelManager.instance.AddPoint(_pointValue);
            }
            else if (BossLevelManager.instance != null)
            {
                BossLevelManager.instance.AddPoint(_pointValue);
            }
        }
        else
        {
            _currentHealth = _maxHealth;
            UIManager.instance.UpdateHealthBarColor(_currentLive);
        }

        _animator.SetBool(AnimationStrings.isDead, true);
    }

    public void AnimationEvent_Die()
    {
        if (_currentLive > 0)
        {
            // Respawn
            Respawn();
        }
        else
        {
            // Finally Die
            Destroy(this.gameObject);
        }
    }

    private void Respawn()
    {
        _isDead = false;
        _animator.SetBool(AnimationStrings.isDead, false);
        _animator.Play("BossCrabbyRevive");
    }

    public virtual void PlayAttackSFX()
    {
    }

    public virtual void PlayDeathSFX()
    {
    }
    #endregion
}