using UnityEngine;

public class EnemyChicken : Enemy
{
    [Header("Chicken Info")]
    [SerializeField, Range(0f, 1f)] private float _aggroDuration = .2f;
    private float _detectionRange;
    private float _aggroTimer;

    protected override void Awake()
    {
        base.Awake();
        InitValues();
    }

    private void InitValues()
    {
        _canMove = false;
        _detectionRange = _playerCheckDistance;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (_isDead) return;

        HandleMovement();
        HandleMovementBlock();
    }

    private void HandleMovement()
    {
        if (!_canMove) return;

        SetVelocityMove();
    }

    private void HandleMovementBlock()
    {
        if (!_isGrounded) return;

        if (!_isGroundInFrontDetected || _isWallDetected)
        {
            _canMove = false;
            SetVelocityZero();
        }
    }

    protected override void Update()
    {
        base.Update();
        if (_isDead) return;

        HandleAggro();
        HandleFlip(_playerTransform.position);
    }

    private void HandleAggro()
    {
        _aggroTimer -= Time.deltaTime;

        if (_isPlayerDetected)
        {
            _canMove = true;
            _aggroTimer = _aggroDuration;
        }

        if (_aggroTimer < 0) _canMove = false;
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

    protected override void Flip()
    {
        base.Flip();
    }
}
