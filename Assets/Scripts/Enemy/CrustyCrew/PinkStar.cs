using UnityEngine;
using System.Collections;

public class PinkStar : BaseEnemy
{
    [Header("Dash Settings")]
    [SerializeField] private float _dashForce = 10f;
    [SerializeField] private float _dashTime = 0.5f;
    public override void PlayAttackSFX()
    {
        base.PlayAttackSFX();
    }

    IEnumerator Dash()
    {
        Vector2 dashDirection;

        if (transform.localScale.x < 0f)
        {
            dashDirection = Vector2.right;
        }
        else
        {
            dashDirection = Vector2.left;
        }

        _rb.linearVelocity = dashDirection * _dashForce;
        yield return new WaitForSeconds(_dashTime);
        _rb.linearVelocity = Vector2.zero;
    }
}

