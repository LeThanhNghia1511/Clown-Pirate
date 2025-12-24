using UnityEngine;

public class CannonBall : Bullet
{
    public override void Animation_Explode()
    {
        AudioManager.instance.PlayCannonExplosionSFX();
        GameObject debris = Instantiate(_debrisBullet, transform.position, Quaternion.identity);
        _moveSpeed = 0f;
    }
    public void Animation_DestroyBullet()
    {
        Destroy(this.gameObject);
    }
}
