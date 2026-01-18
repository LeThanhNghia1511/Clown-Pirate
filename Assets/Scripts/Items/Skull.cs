using UnityEngine;

public class Skull : Item
{
    public override void PlaySFX()
    {
        AudioManager.instance.PlaySFX("skull");
    }
}
