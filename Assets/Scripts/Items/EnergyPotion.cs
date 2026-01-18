using System;
using UnityEngine;

public class EnergyPotion : Item
{
    [SerializeField] private float _energy = 30f;
    public override void ApplyEffect()
    {
        BuffManager.instance.Regen(_energy);
    }
}
