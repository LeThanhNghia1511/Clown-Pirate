using Unity.VisualScripting;
using UnityEngine;

public class Barrel : MonoBehaviour
{
    [SerializeField] private float _maxHealth = 50f;
    [SerializeField] private float _currentHealth;
    [SerializeField] private GameObject _debris;
    [Header("Item")]
    [SerializeField] private int _numberOfItems = 1;
    [SerializeField] private GameObject[] _itemPrefabs = null;
    [Tooltip("The rate this barrel/box is going to drop items")]
    [SerializeField] private float _dropRate = 0.5f; // 50%

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
        AudioManager.instance.PlaySFX("barrelHit");
        _knockback.GetKnocked(_knockBackForce, damageSourcePos);
        if (_currentHealth <= 0) // Die (Break)
        {
            _animator.SetTrigger("Destroyed");
            AudioManager.instance.PlaySFX("barrelBreak");
        }
    }

    public void Animation_Kill()
    {
        if (_debris != null)
        {
            GameObject debris = Instantiate(_debris, transform.position, Quaternion.identity);
        }
        DropItem();
        DestroyBarrel();
    }

    private void DestroyBarrel()
    {
        SwordController sword = GetComponentInChildren<SwordController>();
        if (sword == null)
        {
            Destroy(this.gameObject);
            return;
        }
        sword.transform.SetParent(null);
        sword.DropSword();
        Destroy(this.gameObject);

    }

    private void DropItem()
    {
        if (_itemPrefabs == null || _itemPrefabs.Length == 0) return;
        if (Random.value > _dropRate) return;

        for (int i = 0; i < _numberOfItems; i++)
        {
            int randomIndex = Random.Range(0, _itemPrefabs.Length);
            GameObject selectedPrefab = _itemPrefabs[randomIndex];

            if (selectedPrefab != null)
            {
                GameObject item = Instantiate(selectedPrefab, transform.position, Quaternion.identity);
                float randomX = Random.Range(-5f, 5f);
                float randomY = Random.Range(0.5f, 1f);
                Vector2 force = new Vector2(randomX, randomY);

                Item itemScript = item.GetComponent<Item>();
                if (itemScript != null)
                {
                    itemScript.Spawn(force);
                }
            }
        }
    }
}
