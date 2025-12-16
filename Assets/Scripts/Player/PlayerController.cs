using System.Drawing;
using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _rbPlayer;
    [SerializeField] private float _moveSpeed = 5f;
    private float _moveX;
    [SerializeField] private float _jumpForce = 5f;
    [SerializeField] private Vector2 _boxSize = new Vector2(0.8f, 0.1f); // The size of the box
    [SerializeField] private LayerMask _groundLayer;

    private bool _canMove = true; // Chỉ cho phép di chuyển khi TRUE
    [SerializeField] private float _knockbackPower = 50f;
    [SerializeField] private float _knockbackDuration = 0.2f;

    // Attack Handle
    private bool _hasSword = true;
    private int _currentComboStage = 0;
    private bool _queuedAttack = false;

    [Header("Animator Assets")]
    [SerializeField] private Animator _animator;
    [SerializeField] private RuntimeAnimatorController _controllerWithSword;
    [SerializeField] private RuntimeAnimatorController _controllerNoSword;

    [Header("Thrown Sword")]
    [SerializeField] private GameObject _thrownSwordPrefab; // Kéo Prefab ThrownSword vào đây
    [SerializeField] private Transform _swordSpawnPoint;


    [SerializeField] private GameObject _swordCollider;

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
    }
    private void Update()
    {
        if (GameManager.instance.isPausedMenuShowed == true)
        {
            return;
        }
        HandleMove();
        HandleJump();
        HandleFall();
        HandleAttackInput();
    }

    #region Attack
    private void HandleAttackInput()
    {
        // Normal attack
        if (Input.GetMouseButtonDown(0) && _canMove && _hasSword)
        {
            //    if (_canMove)
            //    {
            //        Debug.Log("can move");
            //    }
            //    if (_hasSword)
            //    {
            //        Debug.Log("has sword");
            //    }
            // Khi bấm nút, không tăng ComboStage ngay lập tức
            //Debug.Log(_currentComboStage);
            // Trường hợp 1: Bắt đầu Combo 1
            _swordCollider.SetActive(true);
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
            _swordCollider.SetActive(false);
            _currentComboStage = 0;

            // ⭐ Báo hiệu cho Animator rằng combo đã kết thúc
            _animator.SetInteger("ComboStage", 0);

            // ⭐ Reset cờ lưu ý định tấn công tiếp theo
            _queuedAttack = false;

            // Tùy chọn: Log để kiểm tra
            Debug.Log("Combo Reset.");
        }
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
        _animator.runtimeAnimatorController = _controllerWithSword;
        _hasSword = true;
        _animator.SetBool("isRunning", false); // Ví dụ: Bật lại trạng thái chạy
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
    //        _rbPlayer.linearVelocity = new Vector2(this.transform.position.x, _jumpForce);
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
        if (Input.GetKeyDown(KeyCode.Space) && CheckGround())
        {
            Debug.Log("Jumping");
            _rbPlayer.AddForce(Vector2.up*_jumpForce);
            AudioManager.instance.PlayJumpSFX();
            _animator.SetBool("isJumping", true);
        }
    }

    private void HandleFall()
    {
        if (!CheckGround() && _rbPlayer.linearVelocity.y < 0) // Khong cham dat va velocity dang giam (Fall)
        {
            Debug.Log("Falling");
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


    public void ApplyKnockback(Vector2 direction)
    {
        // Xác định hướng đẩy lùi ngang: ngược lại với hướng va chạm (direction.x)
        float horizontalPush = -Mathf.Sign(direction.x) * _knockbackPower;

        // ⭐ GIẢM LỰC ĐẨY LÊN: chỉ bằng 1/10 lực đẩy ngang (hoặc dùng giá trị cố định nhỏ)
        float verticalPush = _knockbackPower * 0.1f; // Ví dụ: Giảm từ 0.5f xuống 0.1f

        Vector2 knockbackVector = new Vector2(horizontalPush, verticalPush);

        // ⭐ 1. Khóa điều khiển
        _canMove = false;

        // 2. Thiết lập vận tốc về 0 trước khi đẩy
        _rbPlayer.linearVelocity = Vector2.zero;

        // 3. Áp dụng lực đẩy (Dùng ForceMode2D.Impulse để áp dụng lực ngay lập tức)
        _rbPlayer.AddForce(knockbackVector, ForceMode2D.Impulse);

        // Bắt đầu Coroutine để bật lại điều khiển
        StartCoroutine(KnockbackFreezeRoutine(_knockbackDuration));
    }

    private IEnumerator KnockbackFreezeRoutine(float duration)
    {
        // Chờ hết thời gian đẩy lùi
        yield return new WaitForSeconds(duration);

        // ⭐ Đảm bảo Player vẫn ở trạng thái giật lùi cho đến khi chạm đất
        // Dù _canMove = true, lực hấp dẫn sẽ kéo Player xuống

        _canMove = true; // Bật lại di chuyển sau khi hết thời gian đẩy lùi
    }
    #endregion

    public void LockControlsOnDeath()
    {
        // Khóa di chuyển vĩnh viễn
        _canMove = false;

        // Tùy chọn: Tắt input processing nếu cần thêm (dù _canMove đã làm việc này)
        enabled = false;

        Debug.Log("Player Controls Locked.");
    }
}
