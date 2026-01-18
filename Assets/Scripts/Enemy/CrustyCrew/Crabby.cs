public class Crabby : BaseEnemy
{
    public override void PlayAttackSFX()
    {
        AudioManager.instance.PlaySFX("clamp");
    }
}