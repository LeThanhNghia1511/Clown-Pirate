using System.Drawing;
using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Parameters")]
    [SerializeField] private Rigidbody2D _rbPlayer;
    [SerializeField] private float _moveSpeed = 5f;
    private float _moveX;
    private bool _canMove = true; // Chỉ cho phép di chuyển khi TRUE
    [SerializeField] public float jumpForce = 5f;
    [SerializeField] private Vector2 _boxSize = new Vector2(0.8f, 0.1f); // The size of the box
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private int _maxJump = 2;
    private int _jumpCount = 0;

    // Attack Handle
    private bool _hasSword = true;
    private int _currentComboStage = 0;
    private bool _queuedAttack = false;

    [Header("Animator Assets")]
    [SerializeField] private Animator _animator;
    [SerializeField] private RuntimeAnimatorController _controllerWithSword;
    [SerializeField] private RuntimeAnimatorController _controllerNoSword;

    [Header("Thrown Sword")]
    [SerializeField] private GameObject _thrownSwordPrefab;
    [SerializeField] private Transform _swordSpawnPoint;
    [SerializeField] private GameObject _swordCollider;

    [Header("Visual Effect")]
    [SerializeField] private GameObject _jumpPrefab;
    [SerializeField] private GameObject _groundPrefab;
    [SerializeField] private GameObject _runPrefab;
    [SerializeField] private Transform _dustPosition;
    private Knockback _knockback;

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
    }
    private void Update()
    {
        if (GameManager.instance.isPausedMenuShowed == true)
        {
            return;
        }
        if (CheckGround() && _rbPlayer.linearVelocity.y <= 0.1f) // Khi cham dat va dang roi
        {
            _jumpCount = 0;
        }
        HandleFall();
        if (_knockback.IsBeingKnocked) return;
        HandleMove();
        HandleJump();
        HandleAttackInput();
    }

    #region Attack
    private void HandleAttackInput()
    {
        // Normal attack
        if (Input.GetMouseButtonDown(0) && _canMove && _hasSword)
        {
            // Trường hợp 1: Bắt đầu Combo 1
            if (_currentComboStage == 0)
            {
                _currentComboStage = 1;
                _animator.SetInteger("ComboStage", _currentComboStage);
            }
            // Trường hợp 2: Đang ở giữa Combo (1 hoặc 2)
            else if (_currentComboStage < 3)
            {
                // ⭐ KHÔNG tăng _currentComboStage ngay! Chỉ bật cờ QUEUE.
                _queuedAttack = true;
            }
        }
        // Throw Sword
        if (Input.GetMouseButtonDown(1) && _canMove && _hasSword)
        {
            Debug.Log("ThrowSword");
            _animator.SetTrigger("ThrowSword");
        }
    }

    public void AnimationEvent_TryComboTransition()
    {
        // 1. Nếu người chơi đã bấm nút trong lúc Animation chạy
        if (_queuedAttack)
        {
            _queuedAttack = false; // Tắt cờ Queue

            // ⭐ Tăng ComboStage và chuyển sang Animation tiếp theo
            _currentComboStage++;
            _animator.SetInteger("ComboStage", _currentComboStage);
        }
        // 2. Nếu người chơi KHÔNG bấm nút (Combo bị hủy)
        else if (_currentComboStage != 0)
        {
            // Đặt ComboStage = 0, kích hoạt Transition Attack N -> Idle/Run
            ResetCombo();
        }
    }

    public void AnimationEvent_SwitchToNoSwordVisual()
    {
        _animator.runtimeAnimatorController = _controllerNoSword;
        _hasSword = false;
        _animator.SetBool("isRunning", false); // Ví dụ: Bật lại trạng thái chạy
    }

    private void ResetCombo()
    {
        if (_currentComboStage != 0)
        {
            _currentComboStage = 0;

            // ⭐ Báo hiệu cho Animator rằng combo đã kết thúc
            _animator.SetInteger("ComboStage", 0);

            // ⭐ Reset cờ lưu ý định tấn công tiếp theo
            _queuedAttack = false;

            // Tùy chọn: Log để kiểm tra
            Debug.Log("Combo Reset.");
        }
    }

    public void Animation_PlayAttackSFX()
    {
        AudioManager.instance.PlayAttackSFX();
    }
    // Throw sword
    public void AnimationEvent_ThrowSword()
    {
        // ⭐ 1. Xác định hướng ném (dựa trên hướng nhìn của nhân vật)
        Vector2 throwDirection = (transform.localScale.x > 0) ? Vector2.right : Vector2.left;

        // ⭐ 2. Tạo ra thanh kiếm
        GameObject swordObject = Instantiate(_thrownSwordPrefab,
                                            _swordSpawnPoint.position,
                                            Quaternion.identity);
        swordObject.transform.localScale = new Vector3(transform.localScale.x, 1f, 1f);

        // ⭐ 3. Khởi tạo thanh kiếm và truyền tham chiếu Player
        SwordController swordScript = swordObject.GetComponent<SwordController>();
        if (swordScript != null)
        {
            swordScript.Initialize(throwDirection); // Truyền 'this' để tham chiếu PlayerController
        }
    }

    public void RecoverSword()
    {
        _currentComboStage = 0;
        _queuedAttack = false;

        _animator.runtimeAnimatorController = _controllerWithSword;
        _hasSword = true;

        _animator.SetInteger("ComboStage", 0);
        _animator.SetBool("isRunning", false);
    }

    #endregion

    #region Move
    private void HandleMove()
    {
        // Get input
        _moveX = Input.GetAxisRaw("Horizontal");

        _rbPlayer.linearVelocityX = _moveX *_moveSpeed;
        if (_moveX != 0)
        {
            _animator.SetBool("isRunning", true);
            CreateDust(_runPrefab);
        }
        else
        {
            _animator.SetBool("isRunning", false);
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

    //------ CHECKGROUND USING RAYCAST ------
    // pretty good but with just 1 line in the middle can cause the animation errors when the player object
    // standing at the cliff of the terrain -> can use 3 line (left - middle - right) to fix this or boxcast
    //private void HandleJump()
    //{
    //    if (Input.GetKeyDown(KeyCode.Space) && CheckGround())
    //    {
    //        Debug.Log("Jumping");
    //        _rbPlayer.linearVelocity = new Vector2(this.transform.position.x, jumpForce);
    //    }
    //}

    //private void HandleFall()
    //{
    //    if (!CheckGround() && _rbPlayer.linearVelocity.y < 0) // Khong cham dat va velocity dang giam
    //    {
    //        Debug.Log("Falling");
    //        _animator.SetBool("isFalling", true);
    //        _animator.SetBool("isJumping", false);
    //    }
    //    else if (CheckGround()) // Neu cham dat
    //    {
    //        _animator.SetBool("isFalling", false);
    //        _animator.SetBool("isJumping", false);
    //    }
    //}

    //private bool CheckGround() // Kiem tra cham dat: Cham - true, ko cham - false
    //{
    //    bool output = Physics2D.Raycast(this.transform.position, Vector2.down, 0.2f, _groundLayer);
    //    _animator.SetBool("isJumping", !output);
    //    return output;
    //}


    //------ CHECKGROUND USING BOXCAST (use a box (like retangle or circle instead of a line) ------
    private void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (CheckGround() || _jumpCount < _maxJump)
            {
                _rbPlayer.linearVelocity = new Vector2(_rbPlayer.linearVelocity.x, 0);
                _rbPlayer.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

                // Audio và Anim
                AudioManager.instance.PlayJumpSFX();
                _animator.SetBool("isJumping", true);
                CreateDust(_jumpPrefab);
                _jumpCount++;
                Debug.Log(_jumpCount);
            }
        }
    }

    private void HandleFall()
    {
        if (!CheckGround() && _rbPlayer.linearVelocity.y < 0) // Khong cham dat va velocity dang giam (Fall)
        {
            //Debug.Log("Falling");
            _animator.SetBool("isJumping", false);
            _animator.SetBool("isFalling", true);
        }
        else if (CheckGround()) // Neu cham dat
        {
            _animator.SetBool("isFalling", false);
        }
    }

    private bool CheckGround() // Kiem tra cham dat: Cham - true, ko cham - false
    {
        // Start point of boxcast
        Vector2 startPoint = (Vector2)transform.position + Vector2.down * (_boxSize.y / 2f);
        bool output = Physics2D.BoxCast(startPoint, _boxSize, 0f, Vector2.down, 0.2f, _groundLayer);
        return output;
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
        if (_currentComboStage == 1)
        {
            CameraManager.instance.ShakeCamera(0.5f, 0.2f); // Horizontal
        }
        else CameraManager.instance.ShakeCamera(0.5f, 0.2f, 1); // Vertical
    }

    private void CreateDust(GameObject dustPrefab)
    {
        if (!dustPrefab)
        {
            GameObject dust = Instantiate(dustPrefab, _dustPosition.position, Quaternion.identity);
        }
    }
}
