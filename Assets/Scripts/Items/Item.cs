using UnityEngine;
using System.Collections;

public class Item : MonoBehaviour
{
    [SerializeField] protected GameObject _effect;
    [SerializeField] private bool _isDropable = true;

    public virtual void ApplyEffect()
    {
        // To be overridden by derived classes
    }

    private Rigidbody2D _rb;
    [SerializeField] private float _stopAfterTime = 0.4f;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        if (_isDropable == false)
        {
            StartCoroutine(StopMovementRoutine(0f));
        }
    }

    public void Spawn(Vector2 flyForce)
    {
        if (_isDropable)
        {
            _rb.AddForce(flyForce, ForceMode2D.Impulse);
            StartCoroutine(StopMovementRoutine(_stopAfterTime));
        }
    }

    private IEnumerator StopMovementRoutine(float time)
    {
        yield return new WaitForSeconds(time);

        _rb.linearVelocity = Vector2.zero;
        _rb.bodyType = RigidbodyType2D.Kinematic;

        StartCoroutine(FloatingRoutine());
    }

    public IEnumerator FloatingRoutine()
    {
        Vector3 startPos = transform.position + Vector3.up * 0.3f;
        while (true)
        {
            float newY = startPos.y + Mathf.Sin(Time.time * 2f) * 0.1f;
            transform.position = new Vector3(startPos.x, newY, transform.position.z);
            yield return null;
        }
    }

    public virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null && collision.gameObject.CompareTag("Player"))
        {
            ApplyEffect();
            PlaySFX();
            if (_effect != null)
            {
                Instantiate(_effect, this.transform.position, Quaternion.identity);
            }
            Destroy(gameObject);
        }
    }

    public virtual void PlaySFX()
    {
        AudioManager.instance.PlaySFX("coin");
    }
}
