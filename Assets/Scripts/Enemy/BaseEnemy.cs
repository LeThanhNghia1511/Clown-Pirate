using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class BaseEnemy : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] protected float _maxHealth = 3f;
    [SerializeField] protected float _currentHealth = 0f;

    [Header("Live")]
    [SerializeField] protected int _maxLive = 1;
    [SerializeField] protected int _currentLive = 0;

    [Header("AI Setting")]
    [SerializeField] protected float _moveSpeed = 5f;
    [SerializeField] protected float _moveRange = 10f; // From the left to the right
    private bool _canMove = true;
    [SerializeField] protected float _jumpForce;
    [SerializeField] protected float _groundCheckDistance = 1.2f; // The distance from the pivot to the ground
    [SerializeField] protected float _distanceToPlayer = 2f;
    [SerializeField] protected float _raycastLength = 2f; // The distance from enemy to the wall

    [Header("Score")]
    [SerializeField] protected int _scoreValue = 20;

    [Header("Layers")]
    [SerializeField] protected LayerMask _wallLayer;
    [SerializeField] protected LayerMask _groundLayer;
    [SerializeField] protected LayerMask _playerLayer;

    protected Animator _animator;
    protected Rigidbody2D _rb;
    protected Vector3 _startPosition;
    protected bool _isFacingRight;
    protected bool _isAttacking = false;


    private void Start()
    {
        _animator = this.GetComponent<Animator>();
        _rb = this.GetComponent<Rigidbody2D>();
        _startPosition = this.transform.position;
        _currentHealth = _maxHealth;
        _currentLive = _maxLive;
        _canMove = true;
        if (_animator != null)
        {
            _animator.SetBool("isDead", false);
        }
    }

    private void Update()
    {
        HandleMovement();
        HandleAutoJump();
        UpdateAnimationParameters();
        CheckForPlayer();
    }

    private void HandleMovement()
    {
        _animator.SetBool("isRunning", true);
        float currentX = this.transform.position.x;
        float targetVelocityX = _isFacingRight ? _moveSpeed : -_moveSpeed;

        if (_canMove)
        {
            if (_isFacingRight && currentX >= _startPosition.x + (_moveRange / 2f))
            {
                Flip();
            }
            else if (!_isFacingRight && currentX <= _startPosition.x - (_moveRange / 2f))
            {
                Flip();
            }
            _rb.linearVelocity = new Vector2(targetVelocityX, _rb.linearVelocity.y);
        }
    }

    private void Flip()
    {
        _rb.transform.localScale = new Vector3(_rb.transform.localScale.x * -1, 1f, 1f);
        _isFacingRight = !_isFacingRight;
    }

    #region Jump
    private void HandleJump()
    {
        Debug.Log("Enemy's Jumping");
        // Jump
        _rb.AddForce(Vector2.up * _jumpForce);
    }

    private void HandleAutoJump() // Jump when hit the wall
    {
        Vector2 castStartPos = this.transform.position - Vector3.up * 0.5f;
        Vector2 castDir = _isFacingRight ? Vector2.right : Vector2.left;

        RaycastHit2D _hitTheWall = Physics2D.Raycast(
                                        castStartPos, // Start position of raycast
                                        castDir, // Raycast direction
                                        _raycastLength, // Distance
                                        _wallLayer); // Layer

        if (_hitTheWall.collider != null && CheckGround() && _canMove)
        {
            HandleJump();
        }

        // Show raycast
        //Color rayColor = _hitTheWall.collider != null ? Color.red : Color.green; // The color turn red when hit the wall
        //Debug.DrawRay(
        //    castStartPos,
        //    castDir * _raycastLength,
        //    rayColor,
        //    Time.fixedDeltaTime
        //);
    }

    private bool CheckGround()
    {
        Vector2 castStartPos = this.transform.position - Vector3.up * 0.5f;
        RaycastHit2D _hitGround = Physics2D.Raycast(
                                                    castStartPos,
                                                    Vector2.down,
                                                    _groundCheckDistance,
                                                    _groundLayer);
        // Show raycast
        //Color rayColor = _hitGround.collider != null ? Color.blue : Color.red; // The color turn blue when hit the wall
        //Debug.DrawRay(
        //    castStartPos,
        //    Vector2.down * _groundCheckDistance,
        //    rayColor,
        //    Time.fixedDeltaTime
        //);

        if (_hitGround.collider != null)
        {
            _animator.SetTrigger("Grounded");
            return true;
        }
        else return false;
    }
    #endregion

    private void CheckForPlayer()
    {
        Vector2 castStartPos = this.transform.position;
        Vector2 castDir = _isFacingRight ? Vector2.right : Vector2.left;

        RaycastHit2D _hitPlayer = Physics2D.Raycast(
                                        castStartPos, // Start position of raycast
                                        castDir, // Raycast direction
                                        _distanceToPlayer, // Distance fom enemy to player
                                        _playerLayer); // Layer

        if (_hitPlayer.collider != null && !_isAttacking)
        {
            Debug.Log("Attack Player");
            Attack();
            _isAttacking = true;
        }
        if (_hitPlayer.collider == null)
        {
            _isAttacking = false;
        }

        //    // Show raycast
        //    Color rayColor = _hitPlayer.collider != null ? Color.red : Color.green; // The color turn red when hit the wall
        //Debug.DrawRay(
        //    castStartPos,
        //    castDir * 1f,
        //    rayColor,
        //    Time.fixedDeltaTime
        //);
    }

    private void Attack()
    {
        _animator.SetTrigger("Attack");
    }

    public virtual void TakeDamage(float damage)
    {
        _currentHealth -= damage;
        _animator.SetTrigger("Hit");
        if (_currentHealth <= 0)
        {
            HandleDeath();
        }
    }

    private void HandleDeath()
    {
        _currentLive--;
        if (_currentLive <= 0)
        {
            _canMove = false;
            _animator.SetTrigger("DeadHit");
            _animator.SetBool("isDead", true);
        }
    }

    public void Animation_Kill()
    {
        Debug.Log("Kill");
        LevelManager.instance.AddPoint(_scoreValue);
        Destroy(this.gameObject);
    }

    private void UpdateAnimationParameters()
    {
        float velocityY = _rb.linearVelocityY;
        if (velocityY > 0.1f)
        {
            _animator.SetBool("isJumping", true);
        }
        else if (velocityY <= 0.1f) // Falling
        {
            _animator.SetTrigger("Falling");
            _animator.SetBool("isJumping", false);
        }
    }
}