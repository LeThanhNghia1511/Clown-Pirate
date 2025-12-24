using UnityEngine;

public class Cannon : Shooter
{
    public override void AnimationEvent_Shoot()
    {
        AudioManager.instance.PlayCannonFireSFX();
        base.AnimationEvent_Shoot();
    }
}
