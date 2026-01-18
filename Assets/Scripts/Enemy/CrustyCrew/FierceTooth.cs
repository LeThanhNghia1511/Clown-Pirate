public class FierceTooth : BaseEnemy
{
    public override void PlayAttackSFX()
    {
        AudioManager.instance.PlaySFX("fierceAttack");
    }
}
