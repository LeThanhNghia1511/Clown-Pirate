using UnityEngine;
using System.Collections;

public class HighJumpPotion : Potion
{
    [SerializeField] private float _jumpBoostAmount = 500f;
    [SerializeField] private float _effectTime = 5f;

    public override void ApplyEffect()
    {
        BuffManager.instance.ApplyHighJumpBoost(_jumpBoostAmount, _effectTime);
    }
}
