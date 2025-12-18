using UnityEngine;

public class eAttackCollider : MonoBehaviour
{
    [SerializeField] private float _damage = 10f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null && collision.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(_damage, Vector2.right);
            }
        }
    }
}
