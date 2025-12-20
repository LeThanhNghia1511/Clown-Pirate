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
    [SerializeField] protected int _scoreValue = 20;

    [Header("Layers")]
    [SerializeField] protected LayerMask _wallLayer;
    [SerializeField] protected LayerMask _groundLayer;
    [SerializeField] protected LayerMask _playerLayer;

    protected Animator _animator;
    protected Rigidbody2D _rb;
    protected Vector3 _startPosition;
    protected bool _isFacingRight = false;
    protected bool _isAttacking = false;


    private void Awake()
    {
        _animator = this.GetComponent<Animator>();
        _rb = this.GetComponent<Rigidbody2D>();
        _currentHealth = _maxHealth;
        _currentLive = _maxLive;
        _isFacingRight = false;
        _shootCounter = _shootCooldown;
        if (_animator != null)
        {
            _animator.SetBool("isDead", false);
        }
    }

    public virtual void Update()
    {
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
        _animator.SetTrigger("Shoot");
        _shootCounter = _shootCooldown;
    }

    public void Animation_Shoot()
    {
        Debug.Log("Shoot Player");
        GameObject bullet = Instantiate(_projectilePrefab, _projectilePosition.position, Quaternion.identity);
        Vector2 shootDir = _isFacingRight ? Vector2.right : Vector2.left;
        bullet.GetComponent<Bullet>().Initialize(shootDir);
    }
    #endregion

    #region Take Damage and Death
    public void TakeDamage(float damage)
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
            _animator.SetTrigger("DeadHit");
            _animator.SetBool("isDead", true);
        }
    }

    public void Animation_Kill()
    {
        Debug.Log("Kill");
        LevelManager.instance.AddPoint(_scoreValue);
        GameObject debris = Instantiate(_debrisObject, transform.position, Quaternion.identity);
        Destroy(this.gameObject);
    }
    #endregion
}
