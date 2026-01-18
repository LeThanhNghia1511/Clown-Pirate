using System.Collections;
using UnityEngine;

public class BossEnemy : BaseEnemy
{
    [SerializeField] private GameObject _deathVFXPrefab;
    [Header("Boss skill Infomations")]
    [SerializeField] private float _skillCoolDown = 30f;
    private float _skillCounter = 10f;
    [SerializeField] private GameObject _minionPrefab;
    [SerializeField] private int _minionQuantity = 3;
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private int _bulletQuantity = 5;
    [SerializeField] private float _jumpSlamForce = 15f;
    [SerializeField] private float _slamSpeed = 15f;
    private bool _isAttacking;
    private void Start()
    {
        UIManager.instance.UpdateBossHP(_currentHealth, _maxHealth);
        UIManager.instance.UpdateHealthBarColor(_currentLive);
        _isAttacking = false;
        _isPlayerDetected = false;
    }

    private void FixedUpdate()
    {
        if (_isPlayerDetected && _isDead == false)
        {
            _skillCounter -= Time.fixedDeltaTime;
        }
        if (_skillCounter <= 0)
        {
            UseRandomSkill();
            _skillCounter = _skillCoolDown;
        }
    }

    public override void TakeDamage(float damage, float knockbackForce, Vector2 damageSourcePos)
    {
        if (_isAttacking) return;
        base.TakeDamage(damage, knockbackForce, damageSourcePos);
        UIManager.instance.UpdateBossHP(_currentHealth, _maxHealth);
    }

    public override void AttackLogic()
    {
        if (_attackCounter <= 0f)
        {
            _isAttacking = true;
        }
        base.AttackLogic();
    }

    public override void PlayAttackSFX()
    {
       AudioManager.instance.PlaySFX("clamp");
    }

    public override void PlayDeathSFX()
    {
        AudioManager.instance.PlaySFX("bossRoar");
    }

    public void AnimationEvent_ExitAttack()
    {
        _isAttacking = false;
    }

    public void AnimationEvent_DeathVFX()
    {
        StartCoroutine(DeathEffect());
    }

    IEnumerator DeathEffect()
    {
        int quantity = 10;
        while (quantity > 0)
        {
            Vector3 randomOffset = new Vector3(Random.Range(-3f, 3f), Random.Range(-1f, 4f), 0f);
            Vector3 randomPosition = randomOffset + transform.position;
            GameObject prefab = Instantiate(_deathVFXPrefab, randomPosition, Quaternion.identity);
            quantity--;
            yield return new WaitForSeconds(0.15f);
        }
    }

    void UseRandomSkill()
    {
        int skill = Random.Range(1, 3);
        switch (skill)
        {
            case 1: StartCoroutine(SummonMinion());
                break;
            case 2: StartCoroutine(JumpSlamRoutine());
                break;
            default: StartCoroutine(ShootRoutine());
                break;
        }
    }

    // Skill 1: Summon Minion
    IEnumerator SummonMinion()
    {
        int quantity = _minionQuantity;
        while (quantity > 0)
        {
            GameObject minion = Instantiate(_minionPrefab, transform.position, Quaternion.identity);
            quantity--;
            yield return new WaitForSeconds(0.5f);
        }
    }
    // Skill 2: Jump Slam
    IEnumerator JumpSlamRoutine()
    {
        _animator.Play("BossCrabbyAnticipation");
        yield return new WaitForSeconds(0.25f);
        Vector2 jumpDirection = (_playerTransform.position.x > transform.position.x) ? new Vector2(1, 2) : new Vector2(-1, 2);
        _rb.AddForce(jumpDirection * _jumpSlamForce, ForceMode2D.Impulse);
        _animator.SetTrigger(AnimationStrings.jump);

        yield return new WaitUntil(() => _rb.linearVelocity.y < 0);
        _rb.linearVelocity = new Vector2(0, -_slamSpeed);
        if (_rb.linearVelocityY < 0f)
            AudioManager.instance.PlaySFX("bossLand");
    }
    // Skill 3: Shoot
    IEnumerator ShootRoutine()
    {
        int quantity = _bulletQuantity;
        while (quantity > 0)
        {
            GameObject bullet = Instantiate(_bulletPrefab, transform.position, Quaternion.identity);
            Bullet bulletScript = bullet.GetComponent<Bullet>();
            Vector2 shootDir = _rb.transform.localScale.x > 0f ? Vector2.left : Vector2.right;
            bulletScript.Initialize(shootDir);
            quantity--;
            yield return new WaitForSeconds(0.2f);
        }
    }

}