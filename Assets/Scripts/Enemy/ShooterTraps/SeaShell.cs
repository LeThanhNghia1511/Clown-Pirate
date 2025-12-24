using UnityEngine;
using System.Collections;

public class SeaShell : Shooter
{
    // Attack - Chi co seashell moi co kha nang tan cong gan
    [SerializeField] private float _attackRange = 2f;
    [SerializeField] private float _attackCooldown = 2f;
    private float _attackCounter = 0f;

    private void Start()
    {
        _attackCounter = _attackCooldown;
    }

    public override void Update()
    {
        base.Update();
        _attackCounter -= Time.deltaTime;
    }

    public override void CheckForPlayer()
    {
        Vector2 castStartPos = this.transform.position;
        Vector2 castDir = _isFacingRight ? Vector2.right : Vector2.left;
        // Attack
        RaycastHit2D _hitPlayer = Physics2D.Raycast(
                                        castStartPos, // Start position of raycast
                                        castDir, // Raycast direction
                                        _attackRange, // Distance fom enemy to player
                                        _playerLayer); // Layer

        if (_hitPlayer.collider != null)
        {
            Attack();
        }

        // Shoot
        RaycastHit2D _shootPlayer = Physics2D.Raycast(
                                        castStartPos, // Start position of raycast
                                        castDir, // Raycast direction
                                        _shootRange, // Distance fom enemy to player
                                        _playerLayer); // Layer

        if (_shootPlayer.collider != null && _hitPlayer.collider == null) // Only shoot when cant attack 
        {
            Shoot();
        }
    }

    private void Attack()
    {
        if (_attackCounter > 0f) return;
        // else -> reset counter and attack
        _animator.SetTrigger("Attack");
        AudioManager.instance.PlayBiteSFX();
        _attackCounter = _attackCooldown;
    }

    public override void AnimationEvent_Shoot()
    {
        base.AnimationEvent_Shoot();
    }
}
