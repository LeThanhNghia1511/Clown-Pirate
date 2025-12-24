using System.Runtime.InteropServices;
using UnityEngine;

public class eAttackCollider : MonoBehaviour
{
    [SerializeField] private float _damage = 10f;
    [SerializeField] private float _knockbackForce = 10f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null && collision.CompareTag("Player"))
        {
            PlayerHealth.instance.TakeDamage(_damage, _knockbackForce, transform.position);
        }
    }
}
