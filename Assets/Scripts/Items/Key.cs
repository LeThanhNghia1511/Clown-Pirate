using UnityEngine;

public class Key : Item
{
    public override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null && collision.gameObject.CompareTag("Player"))
        {
            PlayerController.instance.hasKey = true;
            UIManager.instance.ActiveKeyUI();
        }
        base.OnTriggerEnter2D(collision);
    }

    public override void PlaySFX()
    {
        AudioManager.instance.PlaySFX("key");
    }
}
