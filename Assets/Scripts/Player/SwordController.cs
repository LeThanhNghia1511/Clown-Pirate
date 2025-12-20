using UnityEngine;
using System.Collections;

public class SwordController : MonoBehaviour
{
    [SerializeField] private float _speed = 15f;
    [SerializeField] private float _damage = 10f;
    [SerializeField] private float _lifetime = 5f;
    [SerializeField] private LayerMask _wallLayer; // Layer của tường/địa hình

    private Vector2 _direction;
    private bool _isStuck = false;
    private Animator _animator;

    public void Initialize(Vector2 direction)
    {
        // Khởi tạo hướng và Player
        _direction = direction.normalized;
        //this.transform.localScale = new Vector3(transform.localScale.x, 1f, 1f); // Dựa vào hướng nvat để điều chỉnh hướng của thanh kiếm
        _animator = this.GetComponent<Animator>();
    }

    private void Start()
    {
        StartCoroutine(LifetimeTimer());
    }

    private void Update()
    {
        if (!_isStuck) // Move the sword
        {
            transform.Translate(_direction * _speed * Time.deltaTime, Space.World);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Signal");
        // Va chạm với Tường (Ghim Kiếm)
        if (((1 << other.gameObject.layer) & _wallLayer) != 0)
        {
            Debug.Log("Signal Wall");
            Vector2 hitPoint = other.ClosestPoint(transform.position);
            StickToWall(other.transform, hitPoint);
        }

        // Va chạm với Player (Nhặt Kiếm)
        if (other.CompareTag("Player") && _isStuck)
        {
            Debug.Log("Signal Player");
            // Chỉ nhặt khi kiếm đã ghim vào tường
            PickUpSword();
        }

        // Va chạm với enemy
        if (other.CompareTag("Enemy"))
        {
            BaseEnemy enemyHealth = other.GetComponent<BaseEnemy>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(_damage);
            }
        }
    }

    private void StickToWall(Transform wall, Vector2 hitPoint)
    {
        _animator.SetBool("isStuck", true);
        _isStuck = true;
        _speed = 0;
        CameraManager.instance.ShakeCamera(5f, 0.2f);

        // Ngăn chuyển động và vật lý
        float depth = 0.2f;

        // Hướng bay của kiếm (có thể dùng transform.right hoặc transform.up tùy theo trục mũi kiếm)
        Vector2 stickDirection = transform.right;

        // Ép vị trí kiếm = Điểm chạm + (Hướng bay * độ sâu)
        transform.position = hitPoint + (stickDirection * depth);
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }
    }

    private void PickUpSword()
    {
        // Gọi hàm phục hồi Visual with sword trong PlayerController
        PlayerController.instance.RecoverSword();
        _isStuck = false;
        _animator.SetBool("isStuck", false);
        // Hủy GameObject kiếm bay
        Destroy(gameObject);
    }

    public IEnumerator LifetimeTimer()
    {
        // Chờ 10 giây
        yield return new WaitForSeconds(_lifetime);

            Destroy(gameObject);
            Debug.Log("Kiếm đã tự hủy sau " + _lifetime + " giây.");
            PlayerController.instance.RecoverSword();
        
    }
}
