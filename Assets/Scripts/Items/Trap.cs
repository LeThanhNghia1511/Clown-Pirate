using UnityEngine;

public class Trap : MonoBehaviour
{
    [Header("Damage")]
    [SerializeField] private float _enterDamage = 0.5f;
    [SerializeField] private float _knockbackForce = 10f;
    private Vector2 _hitDirection;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (PlayerHealth.instance != null)
            {
                PlayerHealth.instance.TakeDamage(_enterDamage, _knockbackForce, transform.position);
            }
        }
    }
}
