using UnityEngine;

//public class Ship : MonoBehaviour
//{
//    [Header("Bobbing Parameters")]
//    [SerializeField] private float _amplitude = 0.5f;
//    [SerializeField] private float _frequency = 1f;
//    [Header("Movement Parameters")]
//    private Rigidbody2D _rb;
//    [SerializeField] private float _moveSpeed = 5f;
//    [SerializeField] private float _moveRange = 10f;
//    private float _movedDistance = 0f;
//    [SerializeField] private bool _isMovingRight = true;
//    private float _veloX = 0f;
//    private float _veloY = 0f;

//    private void Awake()
//    {
//        _rb = GetComponent<Rigidbody2D>();
//        _veloY = 0f;
//    }
//    private void Update()
//    {
//        Bobbing();
//        _rb.linearVelocity = new Vector2(_veloX, _veloY);
//    }

//    private void Bobbing() // Move the ship up and down
//    {
//        _veloY = Mathf.Cos(Time.time * _frequency) * _frequency * _amplitude;
//    }

//    private void Move() // Move the ship forward
//    {
//        Debug.Log("Ship is moving to the target position");
//        Vector2 moveDirection = _isMovingRight ? Vector2.right : Vector2.up;
//        _veloX = moveDirection.x * _moveSpeed * Time.deltaTime;
//        _movedDistance += _veloX;
//    }

//    private void OnCollisionEnter2D(Collision2D collision)
//    {
//        if (collision != null && collision.gameObject.CompareTag("Player"))
//        {
//            if (_movedDistance < _moveRange)
//            {
//                Move();
//            }
//        }
//    }

//    private void OnCollisionExit2D(Collision2D collision)
//    {
//        if (collision != null && collision.gameObject.CompareTag("Player"))
//        {
//            _veloX = 0f;
//        }
//    }
//}

public class Ship : MonoBehaviour
{
    [Header("Bobbing Parameters")]
    [SerializeField] private float _amplitude = 0.5f;
    [SerializeField] private float _frequency = 1f;

    [Header("Movement Parameters")]
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _moveRange = 10f;
    [SerializeField] private bool _isMovingRight = true;

    private float _movedDistance = 0f;
    private bool _isPlayerOn = false;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _rb.bodyType = RigidbodyType2D.Kinematic;
        _rb.useFullKinematicContacts = true;
    }

    private void FixedUpdate()
    {
        float velY = Mathf.Cos(Time.time * _frequency) * _frequency * _amplitude;
        float velX = 0f;
        if (_isPlayerOn && _movedDistance < _moveRange)
        {
            float direction = _isMovingRight ? 1f : -1f;
            velX = direction * _moveSpeed;

            _movedDistance += Mathf.Abs(velX) * Time.fixedDeltaTime;
        }

        _rb.linearVelocity = new Vector2(velX, velY);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player")) _isPlayerOn = true;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player")) _isPlayerOn = false;
    }
}