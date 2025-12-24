using UnityEngine;

public class DestroyDust : MonoBehaviour
{
    public void AnimationEvent_Destroyed()
    {
        Destroy(this.gameObject);
    }
}
