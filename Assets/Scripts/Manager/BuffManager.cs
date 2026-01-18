using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class BuffManager : MonoBehaviour
{
    private Coroutine _boostCoroutine;
    public static BuffManager instance;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
        _boostCoroutine = null;
    }

    public void ApplyHighJumpBoost(float effectValue, float effectTime)
    {
        if (_boostCoroutine != null)
        {
            StopCoroutine(_boostCoroutine);
            PlayerController.instance.jumpForce -= effectValue;
        }
        _boostCoroutine = StartCoroutine(JumpBoost(effectValue, effectTime));
    }

    private IEnumerator JumpBoost(float effectValue, float effectTime)
    {
        AudioManager.instance.PlaySFX("boost");
        PlayerController.instance.jumpForce += effectValue;
        yield return new WaitForSeconds(effectTime);
        AudioManager.instance.PlaySFX("unboost");
        PlayerController.instance.jumpForce -= effectValue;
    }

    public void Heal(float healAmount)
    {
        AudioManager.instance.PlaySFX("heal");
        PlayerHealth.instance.Heal(healAmount);
    }

    public void Regen(float energy)
    {
        AudioManager.instance.PlaySFX("heal");
        PlayerEnergy.instance.RegenEnergy(energy);
    }
}
