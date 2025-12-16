using UnityEngine;

public class Diamond : MonoBehaviour
{
    [SerializeField] private int _pointValue = 1;
    [SerializeField] private GameObject _diamondEffect;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null && collision.CompareTag("Player"))
        {
            LevelManager.instance.AddPoint(_pointValue);
            AudioManager.instance.PlayCoinSFX();
            Destroy(gameObject);
            GameObject effect = Instantiate(_diamondEffect, this.transform.position, Quaternion.identity);
        }
    }
}
