using UnityEngine;

public class EnemySnail : Enemy
{
    [Header("Snail Info")]
    [SerializeField] private float _maxSpeed = 10f;
    [SerializeField] private EnemySnailBody _snailBpdyPrefab;
    private bool _hasBody = true;
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
        if (!_canMove) return;

        SetVelocityMove();
    }

    private void HandleMovementBlock()
    {
        if (!_isGrounded) return;

        bool canFlipFromLedge = !_isGroundInFrontDetected && _hasBody;
        if (canFlipFromLedge || _isWallDetected)
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

        HandleTurnAround();
    }

    private void HandleTurnAround()
    {
        if (!_canTurnAround) return;
        _canTurnAround = false;

        Flip();
        _idleTimer = _idleDuration;
    }

    public override void Death()
    {
        if (_hasBody)
        {
            _canMove = false;
            _hasBody = false;

            _idleDuration = 0;
            SetVelocityZero();
            _enemyAnim.SetAnimTrigger("Hit");
        }
        else if (!_canMove && !_hasBody)
        {
            _canMove = true;
            _enemyAnim.SetAnimTrigger("Hit");
            _moveSpeed = _maxSpeed;
        }
        else
        {
            base.Death();
        }
    }

    protected override void Flip()
    {
        base.Flip();
        if (!_hasBody) return;

        _enemyAnim.SetAnimTrigger("WallHit");
    }

    public void HandleCreateBody()
    {
        EnemySnailBody newSnailBody = Instantiate(_snailBpdyPrefab, transform.position, Quaternion.identity);

        bool canRotation = Random.Range(0f, 100f) < 50f;
        if (canRotation)
        {
            _deathRotationDirection *= -1;
        }

        float rotationAngle = _deathImpactSeed * _deathRotationDirection;
        newSnailBody.SetupBody(_deathImpactSeed, rotationAngle, _facingDirection);

        float delay = 10f;
        Destroy(newSnailBody, delay);
    }
}
