using UnityEngine;

public class HealthPotion : Potion
{
    [SerializeField] private float healAmount = 20f;
    public override void ApplyEffect() // Heal the player
    {
        PlayerHealth.instance.Heal(healAmount);
    }
}
