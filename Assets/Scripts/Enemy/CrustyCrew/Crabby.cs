using System.Diagnostics;
using UnityEngine;

public class Crabby : BaseEnemy
{
    public override void Attack()
    {
        if (_attackCounter > 0f) return;
        base.Attack();
        AudioManager.instance.PlayClampSFX();
    }
}