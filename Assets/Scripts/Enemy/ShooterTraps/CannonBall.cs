using UnityEngine;

public class CannonBall : Bullet
{
    public override void Animation_Explode()
    {
        AudioManager.instance.PlaySFX("cannonExplosion");
        CameraManager.instance.ShakeCamera(0.7f, 0.3f);
        GameObject debris = Instantiate(_debrisBullet, transform.position, Quaternion.identity);
        _moveSpeed = 0f;
    }
    public void Animation_DestroyBullet()
    {
        Destroy(this.gameObject);
    }
}
