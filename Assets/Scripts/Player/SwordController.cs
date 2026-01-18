using UnityEngine;
using System.Collections;

public class SwordController : MonoBehaviour
{
    [SerializeField] private float _speed = 15f;
    [SerializeField] private float _damage = 10f;
    [SerializeField] public float energyCost = 10f;
    [SerializeField] private float _knockbackForce = 20f;
    [SerializeField] private float _lifetime = 5f;
    [SerializeField] private LayerMask _wallLayer; // Layer của tường/địa hình

    private Vector2 _direction;
    private bool _isStuck = false;
    private bool _isDealStickDamaged = false;
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
        StartCoroutine(LifetimeTimer(_lifetime));
        _isStuck = false;
        _isDealStickDamaged = false;
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
        if (((1 << other.gameObject.layer) & _wallLayer) != 0)
        {
            if (transform.parent != null) return;
            Vector2 hitPoint = other.ClosestPoint(transform.position);
            StickToWall(other.transform, hitPoint);
            gameObject.transform.SetParent(other.transform);
        }

        if (other.CompareTag("Player") && _isStuck)
        {
            PickUpSword();
        }

        // Va chạm với enemy
        if (other.CompareTag("Enemy"))
        {
            BaseEnemy enemyHealth = other.GetComponent<BaseEnemy>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(_damage, _knockbackForce, transform.position);
                CameraManager.instance.ShakeCamera(0.5f, 0.3f);
            }
        }

        if (other.CompareTag("Shooter"))
        {
            Shooter enemyHealth = other.GetComponent<Shooter>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(_damage, _knockbackForce, transform.position);
                CameraManager.instance.ShakeCamera(0.5f, 0.3f);
            }
        }

        if (other.CompareTag("Barrel"))
        {
            if (transform.parent != null && _isDealStickDamaged == true) return;
            Barrel enemyHealth = other.GetComponent<Barrel>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(_damage, _knockbackForce, transform.position);
                CameraManager.instance.ShakeCamera(0.5f, 0.3f);
                _isDealStickDamaged = false;
            }
        }
    }

    private void StickToWall(Transform wall, Vector2 hitPoint)
    {
        _animator.SetBool(AnimationStrings.isStuck, true);
        AudioManager.instance.PlaySFX("swordImpact");
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        DialogueEvent.Trigger(player, "Interrogation");
        _isStuck = true;
        _speed = 0;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            float offsetDepth = -0.1f;
            transform.position = hitPoint + _direction * offsetDepth;

            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        CameraManager.instance.ShakeCamera(1f, 0.2f);
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

    public void DropSword()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 2f;
        float randomX = Random.Range(-2f, 2f);
        rb.AddForce(new Vector2(randomX, 2f), ForceMode2D.Impulse);
        _animator.SetBool(AnimationStrings.isStuck, false);
        StartCoroutine(FloatingRoutine());
        StartCoroutine(LifetimeTimer(2f));
    }

    public IEnumerator FloatingRoutine()
    {
        yield return new WaitForSeconds(1f);
        Vector3 startPos = transform.position + Vector3.up * 0.3f;
        while (true)
        {
            float newY = startPos.y + Mathf.Sin(Time.time * 2f) * 0.1f;
            transform.position = new Vector3(startPos.x, newY, transform.position.z);
            yield return null;
        }
    }

    public IEnumerator LifetimeTimer(float lifetime = 15f)
    {
        // Chờ 10 giây
        yield return new WaitForSeconds(lifetime);
        Destroy(gameObject);
        PlayerController.instance.RecoverSword();
    }
}
