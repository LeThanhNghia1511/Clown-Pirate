using UnityEngine;

public class Diamond : Item
{
    //[SerializeField] private GameObject _effect;
    [SerializeField] private int _pointValue = 1;

    public override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null && collision.gameObject.CompareTag("Player"))
        {
            AudioManager.instance.PlaySFX("coin");
            if (LevelManager.instance != null)
                LevelManager.instance.AddPoint(_pointValue);
            if (BossLevelManager.instance != null)
                BossLevelManager.instance.AddPoint(_pointValue);
            Destroy(gameObject);
            GameObject effect = Instantiate(_effect, this.transform.position, Quaternion.identity);
        }
    }
}
