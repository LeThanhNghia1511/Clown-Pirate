using UnityEngine;

public class pAttackCollider : MonoBehaviour
{
    [SerializeField] private float _damage = 10f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null && collision.CompareTag("Enemy"))
        {
            BaseEnemy enemy = collision.GetComponent<BaseEnemy>();
            if (enemy != null)
            {
                Debug.Log("Hit Enemy");
                enemy.TakeDamage(_damage);
            }
        }
        if (collision != null && collision.CompareTag("Shooter"))
        {
            Shooter shooter = collision.GetComponent<Shooter>();
            if (shooter != null)
            {
                Debug.Log("Hit Shooter");
                shooter.TakeDamage(_damage);
            }
        }
    }
}
