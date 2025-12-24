public class FierceTooth : BaseEnemy
{
    public override void Attack()
    {
        if (_attackCounter > 0f) return;
        base.Attack();
        AudioManager.instance.PlayBiteSFX();
    }
}
