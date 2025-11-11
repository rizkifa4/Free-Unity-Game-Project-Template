using UnityEngine;

public class EnemyTrunk : Enemy
{
    [Header("Trunk Info")]
    [SerializeField, Range(0f, 10f)] private float _bulletSpeed = 10f;
    [SerializeField, Range(0f, 5f)] private float _attackCooldown = 2f;
    [SerializeField] private Transform _gunPoint;
    [SerializeField] private EnemyBullet _bulletPrefab;
    private float _lastTimeAttacked;
    private bool _canAttack;
    private bool _canTurnAround;

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (_isDead) return;

        HandleMovement();
        HandleMovementBlock();
    }

    private void HandleMovement()
    {
        if (_idleTimer > 0) return;

        SetVelocityMove();
    }

    private void HandleMovementBlock()
    {
        if (!_isGrounded) return;

        if (!_isGroundInFrontDetected || _isWallDetected)
        {
            SetVelocityZero();
            RequestTurnAround();
        }
    }

    private void RequestTurnAround()
    {
        if (_canTurnAround) return;
        _canTurnAround = true;
    }

    protected override void Update()
    {
        base.Update();
        if (_isDead) return;

        HandleLastTimeAttack();
        HandleAttack();
        HandleTurnAround();
    }

    private void HandleTurnAround()
    {
        if (!_canTurnAround) return;
        _canTurnAround = false;

        Flip();
        _idleTimer = _idleDuration;
    }

    private void HandleLastTimeAttack()
    {
        if (!_isPlayerDetected) return;

        float nextTimeAttack = _lastTimeAttacked + _attackCooldown;
        _canAttack = Time.time >= nextTimeAttack;
    }

    private void HandleAttack()
    {
        if (!_canAttack) return;

        _idleTimer = _idleDuration + _attackCooldown;
        _lastTimeAttacked = Time.time;
        _enemyAnim.SetAnimTrigger("Attack");
    }

    public void HandleShootBullet()
    {
        EnemyBullet newBullet = Instantiate(_bulletPrefab, _gunPoint.position, Quaternion.identity);

        Vector2 bulletVelocity = new(_facingDirection * _bulletSpeed, 0f);
        newBullet.SetVelocity(bulletVelocity);

        if (_facingDirection == 1) newBullet.SpriteFlip();

        float delay = 10f;
        Destroy(newBullet.gameObject, delay);
    }
}
