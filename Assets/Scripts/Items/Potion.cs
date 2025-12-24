using UnityEngine;
using System.Collections;

public class Potion : MonoBehaviour
{
    [SerializeField] protected GameObject _potionEffect;

    public virtual void ApplyEffect()
    {
        // To be overridden by derived classes
    }

    private Rigidbody2D _rb;
    [SerializeField] private float _stopAfterTime = 0.4f;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    public void Spawn(Vector2 flyForce)
    {
        _rb.AddForce(flyForce, ForceMode2D.Impulse);

        StartCoroutine(StopMovementRoutine());
    }

    private IEnumerator StopMovementRoutine()
    {
        yield return new WaitForSeconds(_stopAfterTime);

        _rb.linearVelocity = Vector2.zero;
        _rb.bodyType = RigidbodyType2D.Kinematic;

        StartCoroutine(FloatingRoutine());
    }

    private IEnumerator FloatingRoutine()
    {
        Vector3 startPos = transform.position + Vector3.up * 0.3f;
        while (true)
        {
            float newY = startPos.y + Mathf.Sin(Time.time * 2f) * 0.1f;
            transform.position = new Vector3(startPos.x, newY, transform.position.z);
            yield return null;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision != null && collision.gameObject.CompareTag("Player"))
        {
            ApplyEffect();
            if (_potionEffect != null)
            {
                Instantiate(_potionEffect, this.transform.position, Quaternion.identity);
            }
            Destroy(gameObject);
        }
    }
}
