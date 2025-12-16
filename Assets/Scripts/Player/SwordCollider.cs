using UnityEngine;

public class SwordCollider : MonoBehaviour
{
    [SerializeField] private float _playerDamage;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null && collision.CompareTag("Enemy"))
        {
            Destroy(collision.gameObject);
            collision.GetComponent<Crabby>().TakeDamage(_playerDamage);
        }
    }
}
