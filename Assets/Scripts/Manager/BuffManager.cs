using System.Collections;
using UnityEngine;

public class BuffManager : MonoBehaviour
{
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
    }

    public void ApplyHighJumpBoost(float effectValue, float effectTime)
    {
        StartCoroutine(JumpBoost(effectValue, effectTime));
    }

    private IEnumerator JumpBoost(float effectValue, float effectTime)
    {
        PlayerController.instance.jumpForce += effectValue;
        yield return new WaitForSeconds(effectTime);
        PlayerController.instance.jumpForce -= effectValue;
    }
}
