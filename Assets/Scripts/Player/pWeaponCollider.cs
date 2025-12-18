using UnityEngine;

public class pWeaponCollider : MonoBehaviour
{
    [SerializeField] private float _damage = 10f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Debug.Log("Attack enemy");
            BaseEnemy enemy = collision.GetComponent<BaseEnemy>();
            if (enemy != null)
            {
                Debug.Log("Hit Enemy");
                enemy.TakeDamage(_damage);
            }
        }
    }
}
