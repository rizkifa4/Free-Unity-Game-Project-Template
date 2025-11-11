using UnityEngine;

public class EnemyPlant : Enemy
{
    [Header("Plant Info")]
    [SerializeField, Range(0f, 10f)] private float _bulletSpeed = 6f;
    [SerializeField, Range(0f, 5f)] private float _attackCooldown = 1.5f;
    [SerializeField] private Transform _gunPoint;
    [SerializeField] private EnemyBullet _bulletPrefab;
    private float _lastTimeAttacked;
    private bool _canAttack;
    private float _detectionRange = 3f;

    protected override void Awake()
    {
        base.Awake();
        _detectionRange = _playerCheckDistance;
    }

    protected override void Update()
    {
        base.Update();
        if (_isDead) return;

        HandleTimeAttack();
        HandleAttack();
        HandleFlip(_playerTransform.position);
    }

    private void HandleTimeAttack()
    {
        if (!_isPlayerDetected) return;

        float nextTimeAttack = _lastTimeAttacked + _attackCooldown;
        _canAttack = Time.time >= nextTimeAttack;
    }

    private void HandleAttack()
    {
        if (!_canAttack) return;

        _lastTimeAttacked = Time.time;
        _enemyAnim.SetAnimTrigger("Attack");
    }

    public void HandleShootBullet()
    {
        EnemyBullet newEnemyBullet = Instantiate(_bulletPrefab, _gunPoint.position, Quaternion.identity);
        Vector2 bulletVelocity = new(_facingDirection * _bulletSpeed, 0);
        newEnemyBullet.SetVelocity(bulletVelocity);
    }

    protected override void HandleIdleMoveAnimation()
    {
        // Plant enemy does not have idle/move animation
        // So this method is intentionally left blank
    }

    private bool IsPlayerInRangeForFlip(Vector2 playerPos)
    {
        Vector2 enemyPos = transform.position;

        float distance = (playerPos - enemyPos).sqrMagnitude;
        if (distance > _detectionRange * _detectionRange) return false;

        return true;
    }

    private bool ShouldFlipBasedOnPlayer(Vector2 playerPos)
    {
        if (!IsPlayerInRangeForFlip(playerPos)) return false;

        Vector2 enemyPos = transform.position;

        bool isPlayerOnLeft = playerPos.x < enemyPos.x && _facingRight;
        bool isPlayerOnRight = playerPos.x > enemyPos.x && !_facingRight;

        return isPlayerOnLeft || isPlayerOnRight;
    }

    protected override void HandleFlip(Vector2 xVector)
    {
        if (ShouldFlipBasedOnPlayer(xVector))
        {
            Flip();
        }
    }
}
