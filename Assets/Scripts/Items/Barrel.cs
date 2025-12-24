using UnityEngine;

public class Barrel : MonoBehaviour
{
    [SerializeField] private float _maxHealth = 50f;
    [SerializeField] private float _currentHealth;
    [SerializeField] private GameObject _debris;
    [SerializeField] private GameObject _itemPrefab = null;

    private Animator _animator;
    private Knockback _knockback;

    private void Start()
    {
        _currentHealth = _maxHealth;
        _animator = GetComponent<Animator>();
        _knockback = GetComponent<Knockback>();
    }

    public void TakeDamage(float damage, float _knockBackForce, Vector2 damageSourcePos)
    {
        _currentHealth -= damage;
        _animator.SetTrigger("Hit");
        AudioManager.instance.PlayBarrelHitSFX();
        _knockback.GetKnocked(_knockBackForce, damageSourcePos);
        if (_currentHealth <= 0) // Die (Break)
        {
            _animator.SetTrigger("Destroyed");
            AudioManager.instance.PlayBarrelBreakSFX();
        }
    }

    public void Animation_Kill()
    {
        if (_debris != null)
        {
            GameObject debris = Instantiate(_debris, transform.position, Quaternion.identity);
        }
        if (_itemPrefab != null) // Drop item
        {
            GameObject itemPrefab = Instantiate(_itemPrefab, transform.position, Quaternion.identity);
            Potion potionScript = itemPrefab.GetComponent<Potion>();
            potionScript.Spawn(Vector2.right*5f);
        }
        Destroy(this.gameObject);
    }
}
