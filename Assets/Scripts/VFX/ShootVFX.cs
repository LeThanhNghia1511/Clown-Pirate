using UnityEngine;

public class ShootVFX : MonoBehaviour
{
    public void AnimationEvent_EndEffect()
    {
        Destroy(gameObject);
    }
}
