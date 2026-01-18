using UnityEngine;

public class Chest : MonoBehaviour
{
    [SerializeField] private GameObject _keyPromptUI;
    [SerializeField] private GameObject _padlock;
    [SerializeField] private bool _needsKey = true;

    [Header("Item Drops")]
    [SerializeField] private GameObject[] _itemPrefabs;
    [SerializeField] private int _numberOfItems = 5;

    private bool _isPlayerInRange;
    private bool _isOpened;
    private Animator _animator;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        if (_keyPromptUI != null) _keyPromptUI.SetActive(false);
    }

    private void Update()
    {
        if (_isPlayerInRange && !_isOpened && Input.GetKeyDown(KeyCode.E))
        {
            OpenChest();
        }
    }

    private void OpenChest()
    {
        if (_needsKey && !PlayerController.instance.hasKey)
        {
            Debug.Log("Không có chìa khóa!");
            return;
        }

        _isOpened = true;
        _animator.SetTrigger(AnimationStrings.open);
        EjectPadlock();
        AudioManager.instance.PlaySFX("openChest");
        if (_keyPromptUI != null) _keyPromptUI.SetActive(false); 

        if (_needsKey) PlayerController.instance.hasKey = false;

        Debug.Log("Rương đã mở!");
        Invoke("DropItem", 0.5f);
    }

    private void DropItem()
    {
        if (_itemPrefabs == null || _itemPrefabs.Length == 0) return;

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

    private void EjectPadlock()
    {
        if (_padlock != null)
        {
            _padlock.SetActive(true);
            float randomX = Random.Range(-8f, 8f);
            float randomY = Random.Range(4f, 6f);
            Vector2 ejectForce = new Vector2(randomX, randomY);
            Rigidbody2D rb = _padlock.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.AddForce(ejectForce, ForceMode2D.Impulse);
                rb.AddTorque(10f, ForceMode2D.Impulse);
            }
            Destroy(_padlock, 1.5f);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !_isOpened)
        {
            _isPlayerInRange = true;
            if (_keyPromptUI != null) _keyPromptUI.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _isPlayerInRange = false;
            if (_keyPromptUI != null) _keyPromptUI.SetActive(false);
        }
    }
}