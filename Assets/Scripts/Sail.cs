using UnityEngine;

public class Sail : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _shipRigidbody;
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (_shipRigidbody.linearVelocity.x >= 0.1f)
        {
            Debug.Log("Moving");
            _animator.SetBool("isMoving", true);
        }
        else
        {
            Debug.Log("Stop Moving");
            _animator.SetBool("isMoving", false);
        }
    }
}
