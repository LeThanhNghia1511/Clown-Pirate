using UnityEngine;
using System.Collections;

public class Knockback : MonoBehaviour
{
    private Rigidbody2D _rb;
    [SerializeField] private float _knockbackTime = 0.2f; // Thời gian bị khựng khi văng
    public bool IsBeingKnocked { get; private set; }

    private void Awake()
    {
        if (_rb == null) _rb = GetComponent<Rigidbody2D>();
    }

    public void GetKnocked(float thrust, Vector2 damageSourcePos)
    {
        if (IsBeingKnocked) return;
        Vector2 difference = (Vector2)transform.position - damageSourcePos;
        Vector2 force = (difference.normalized + Vector2.up * 0.7f).normalized * thrust;

        StartCoroutine(KnockRoutine(force));
    }

    private IEnumerator KnockRoutine(Vector2 force)
    {
        IsBeingKnocked = true;

        _rb.linearVelocity = Vector2.zero;
        _rb.AddForce(force, ForceMode2D.Impulse);

        yield return new WaitForSeconds(_knockbackTime);

        _rb.linearVelocity = Vector2.zero;
        IsBeingKnocked = false;
    }
}
