using UnityEngine;

public class Dialogue : MonoBehaviour
{
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        float parentX = transform.parent.localScale.x;
        if (parentX < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
    }

    private void OnEnable()
    {
        DialogueEvent.OnRequestDialogue += ShowDialogue;
    }

    private void OnDisable()
    {
        DialogueEvent.OnRequestDialogue -= ShowDialogue;
    }

    private void ShowDialogue(GameObject target, string animName)
    {
        if (target == transform.parent.gameObject)
        {
            _animator.SetTrigger(animName);
        }
    }

    public void AnimationEvent_GameOver() // Gameover when the deadDialogue close
    {
        GameManager.instance.LoseGame();
        Destroy(this.gameObject);
    }
}
