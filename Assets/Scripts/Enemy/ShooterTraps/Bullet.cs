using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{
    [SerializeField] protected float _moveSpeed = 20f;
    [SerializeField] private float _damage = 10f;
    [SerializeField] private float _lifeTime = 10f;

    [SerializeField] protected GameObject _debrisBullet;
    private Animator _animator;

    private Vector3 _direction;
    public void Initialize(Vector2 direction)
    {
        _direction = direction.normalized;
        this.transform.localScale = new Vector3(direction.x, 1f, 1f);
    }

    private void Awake()
    {
        _animator = this.GetComponent<Animator>();
    }

    private void Update()
    {
        MoveBullet();
        _lifeTime -= Time.deltaTime;
        if (_lifeTime <= 0f)
        {
            Destroy(gameObject);
        }
    }

    // Gay sat thuong
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null && collision.gameObject.CompareTag("Player"))
        {
            _animator.SetTrigger("Explode");
            PlayerHealth.instance.TakeDamage(_damage);
        }
        if (collision != null && collision.CompareTag("AttackCollider"))
        {
            _animator.SetTrigger("Explode");
        }
    }

    public void MoveBullet()
    {
        // Bay thẳng
        this.transform.position += _direction * _moveSpeed * Time.deltaTime;
    }

    public virtual void Animation_Explode()
    {
        Debug.Log("Instantiate debris bullet");
        GameObject debris = Instantiate(_debrisBullet, transform.position, Quaternion.identity);
        _moveSpeed = 0f;
        Destroy(gameObject);
    }
}
