using UnityEngine;

public class Cannon : Shooter
{
    [SerializeField] private GameObject _shootEffect;
    [SerializeField] private Transform _vfxPosition;
    public override void AnimationEvent_Shoot()
    {
        AudioManager.instance.PlaySFX("cannonFire");
        GameObject vfx = Instantiate(_shootEffect, _vfxPosition.position, Quaternion.identity);
        base.AnimationEvent_Shoot();
    }

    public override void PlayHitSFX()
    {
        AudioManager.instance.PlaySFX("cannonHit");
    }
}
