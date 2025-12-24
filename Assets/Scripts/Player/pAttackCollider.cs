using UnityEngine;

public class pAttackCollider : MonoBehaviour
{
    [SerializeField] private float _damage = 10f;
    [SerializeField] private float _knockbackForce = 20f;
    [SerializeField] private Transform _playerPosition;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null && collision.CompareTag("Enemy"))
        {
            BaseEnemy enemy = collision.GetComponent<BaseEnemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(_damage, _knockbackForce, transform.position);
            }
        }
        if (collision != null && collision.CompareTag("Shooter"))
        {
            Shooter shooter = collision.GetComponent<Shooter>();
            if (shooter != null)
            {
                shooter.TakeDamage(_damage, _knockbackForce, _playerPosition.position);
            }
        }

        if (collision != null && collision.CompareTag("Barrel"))
        {
            Barrel shooter = collision.GetComponent<Barrel>();
            if (shooter != null)
            {
                shooter.TakeDamage(_damage, _knockbackForce, _playerPosition.position);
            }
        }
    }
}
