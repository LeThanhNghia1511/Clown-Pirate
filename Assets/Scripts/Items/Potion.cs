using UnityEngine;

public class Potion : MonoBehaviour
{
    [SerializeField] protected GameObject _potionEffect;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null && collision.CompareTag("Player"))
        {
            ApplyEffect();
            if (_potionEffect != null)
            {
                Instantiate(_potionEffect, this.transform.position, Quaternion.identity);
            }
            Destroy(gameObject);
        }
    }

    public virtual void ApplyEffect()
    {
        // To be overridden by derived classes
    }
}
