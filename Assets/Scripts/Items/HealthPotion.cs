using UnityEngine;

public class HealthPotion : Item
{
    [SerializeField] private float _healAmount = 20f;
    public override void ApplyEffect() // Heal the player
    {
        BuffManager.instance.Heal(_healAmount);
    }
}
