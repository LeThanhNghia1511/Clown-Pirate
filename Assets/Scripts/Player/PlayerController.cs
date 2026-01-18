using System.Drawing;
using UnityEngine;
using System.Collections;
using Unity.VisualScripting;
using System.Runtime.CompilerServices;

public class PlayerController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Rigidbody2D _rbPlayer;
    [SerializeField] private Animator _animator;
    private Knockback _knockback;
    [Header("Movement & Jump")]
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] public float jumpForce = 5f; // public for buff Manager
    [SerializeField] private int _maxJump = 2;
    private float _moveX;
    private int _jumpCount = 0;
    private bool _canMove = true;
    private bool _isGroundedLastFrame = false;
    [Header("Ground Check")]
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private Vector2 _boxSize = new Vector2(0.8f, 0.1f);
    [Header("Attack Logic")]
    private bool _hasSword = true;
    private int _airAttackCount = 0;
    [Header("Sword Management")]
    [SerializeField] private GameObject _thrownSwordPrefab;
    [SerializeField] private Transform _swordSpawnPoint;
    [SerializeField] private GameObject _swordCollider;
    [SerializeField] private RuntimeAnimatorController _controllerWithSword;
    [SerializeField] private RuntimeAnimatorController _controllerNoSword;
    [Header("Visual Effects (VFX)")]
    [SerializeField] private Animator _attackVFXAnimator;
    [SerializeField] private Animator _airAttackVFXAnimator;
    [SerializeField] private GameObject _jumpDust;
    [SerializeField] private GameObject _groundDust;
    [SerializeField] private GameObject _runDust;
    [SerializeField] public bool hasKey = false; // For open chest

    // Singleton for playerController
    public static PlayerController instance { get; private set; }
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
        _knockback = GetComponent<Knockback>();
        _isGroundedLastFrame = false;
        _airAttackCount = 0;
        hasKey = false;
    }

    private void Update()
    {
        if (GameManager.instance.isPausedMenuShowed == true)
        {
            return;
        }
        if (_knockback.IsBeingKnocked) return;
        HandleMove();
        HandleJump();
        HandleFall();
        HandleAttackInput();
    }

    #region Movement & Jump
    private void HandleMove()
    {
        // Get input
        _moveX = Input.GetAxisRaw("Horizontal");
        // Move
        _rbPlayer.linearVelocityX = _moveX *_moveSpeed;
        if (_moveX != 0)
        {
            _animator.SetBool(AnimationStrings.isRunning, true);
        }
        else
        {
            _animator.SetBool(AnimationStrings.isRunning, false);
        }
        // Flip player
        FlipPlayer(_moveX);
    }

    private void FlipPlayer(float input)
    {
        if (input >= 1)
        {
            this.transform.localScale = new Vector3(1f, this.transform.localScale.y, this.transform.localScale.z);
        }
        else if (input <= -1)
        {
            this.transform.localScale = new Vector3(-1f, this.transform.localScale.y, this.transform.localScale.z);
        }
    }

    private void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (CheckGround() || _jumpCount < _maxJump)
            {
                _rbPlayer.linearVelocity = new Vector2(_rbPlayer.linearVelocity.x, 0); // Reset position for the next jump
                _rbPlayer.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                _jumpCount++;

                // SFX & VFX
                AudioManager.instance.PlaySFX("jump");
                _animator.SetTrigger(AnimationStrings.jump);
                _animator.SetBool(AnimationStrings.isGrounded, false);
                CreateDust(_jumpDust);
            }
        }
    }

    private void HandleFall()
    {
        if (!CheckGround() && _rbPlayer.linearVelocity.y < 0) // Khong cham dat va velocity dang giam (Fall)
        {
            _animator.SetTrigger(AnimationStrings.fall);
            _isGroundedLastFrame = false;
        }
    }

    private bool CheckGround() // Kiem tra cham dat: Cham - true, ko cham - false
    {
        // Start point of boxcast
        Vector2 startPoint = (Vector2)transform.position + Vector2.down * (_boxSize.y / 2f);
        bool isGrounded = Physics2D.BoxCast(startPoint, _boxSize, 0f, Vector2.down, 0.2f, _groundLayer);
        if (isGrounded && !_isGroundedLastFrame)
        {
            OnJustLand();
        }
        _isGroundedLastFrame = isGrounded;
        return isGrounded;
    }

    private void OnJustLand()
    {
        CreateDust(_groundDust);
        _airAttackCount = 0;
        _jumpCount = 0;
        _animator.ResetTrigger(AnimationStrings.fall);
        _animator.SetBool(AnimationStrings.isGrounded, true);
    }
    #endregion

    #region Attack
    private void HandleAttackInput()
    {
        // Melee Attack
        if (Input.GetMouseButtonDown(0) && _canMove && _hasSword)
        {
            if (CheckGround()) // Attack (on the ground)
            {
                _animator.SetTrigger(AnimationStrings.attack);
                _attackVFXAnimator.SetTrigger(AnimationStrings.attack);
            }
            else // Air Attack
            {
                if (_airAttackCount >= 2) return;
                Debug.Log(_airAttackCount);
                _airAttackCount++;
                _animator.SetTrigger(AnimationStrings.attack);
                _airAttackVFXAnimator.SetTrigger(AnimationStrings.attack);
                StartCoroutine(AirFreezeRoutine(0.1f));
            }
        }
        // Throw Sword
        if (Input.GetMouseButtonDown(1) && _canMove && _hasSword)
        {
            if (!PlayerEnergy.instance.HaveEnergy()) return;
            _animator.SetTrigger(AnimationStrings.throwSword);
        }
    }

    private IEnumerator AirFreezeRoutine(float duration)
    {
        float originalGravity = _rbPlayer.gravityScale;
        _rbPlayer.gravityScale = 0;
        _rbPlayer.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(duration);

        _rbPlayer.gravityScale = originalGravity;
    }

    public void AnimationEvent_SwitchToNoSwordVisual()
    {
        _animator.runtimeAnimatorController = _controllerNoSword;
        _hasSword = false;
        _animator.SetBool(AnimationStrings.isRunning, false); // Bật lại trạng thái chạy
    }

    public void Animation_PlayAttackSFX()
    {
        AudioManager.instance.PlaySFX("attack");
    }
    // Throw sword
    public void AnimationEvent_ThrowSword()
    {
        Vector2 throwDirection = (transform.localScale.x > 0) ? Vector2.right : Vector2.left;

        GameObject swordObject = Instantiate(_thrownSwordPrefab,
                                            _swordSpawnPoint.position,
                                            Quaternion.identity);
        swordObject.transform.localScale = new Vector3(transform.localScale.x, 1f, 1f);
        float cost = swordObject.GetComponent<SwordController>().energyCost;
        PlayerEnergy.instance.LoseEnergy(cost);

        SwordController swordScript = swordObject.GetComponent<SwordController>();
        if (swordScript != null)
        {
            swordScript.Initialize(throwDirection);
        }
    }

    public void RecoverSword()
    {
        _animator.runtimeAnimatorController = _controllerWithSword;
        _hasSword = true;
        _animator.SetBool(AnimationStrings.isGrounded, true);
    }

    #endregion

    public void LockControlsOnDeath()
    {
        // Khóa di chuyển vĩnh viễn
        _canMove = false;
        _rbPlayer.linearVelocity = Vector2.zero;

        // Tùy chọn: Tắt input processing nếu cần thêm (dù _canMove đã làm việc này)
        enabled = false;

        Debug.Log("Player Controls Locked.");
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision != null &&  collision.gameObject.CompareTag("Water"))
        {
            Debug.Log("Got wet");
            _jumpCount = _maxJump;
            PlayerHealth.instance.Drown();
        }
    }

    public void AnimationEvent_ShakingWhenAttack()
    {
        //CameraManager.instance.ShakeCamera(0.5f, 0.2f); // Horizontal
        CameraManager.instance.ShakeCamera(0.5f, 0.2f, 1); // Vertical
    }

    private void CreateDust(GameObject dustPrefab)
    {
        if (dustPrefab != null)
        {
            GameObject dust = Instantiate(dustPrefab, _rbPlayer.position, Quaternion.identity);
            if (transform.localScale.x < 0)
            {
                dust.transform.localScale = new Vector3(-1, 1, 1);
            }
        }
    }

    public void AnimationEvent_RunDust()
    {
        CreateDust(_runDust);
    }    
}
