using UnityEngine;

public static class DialogueEvent
{
    public static System.Action<GameObject, string> OnRequestDialogue;

    public static void Trigger(GameObject target, string animName)
    {
        OnRequestDialogue?.Invoke(target, animName);
    }
}
