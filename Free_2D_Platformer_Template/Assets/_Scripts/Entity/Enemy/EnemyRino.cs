using UnityEngine;

public class EnemyRino : Enemy
{
    [Header("Rino Info")]
    [SerializeField, Range(0f, 10f)] private float _maxSpeed = 10f;
    [SerializeField, Range(0f, 1f)] private float _speedUpRate = .6f;
    [SerializeField, Range(0f, 5f)] private float _detectionRange = 3f;
    [Space]
    [SerializeField, Range(0f, 10f)] private float _impactXPower = 7f;
    [SerializeField, Range(0f, 10f)] private float _impactYPower = 9f;

    private bool _isOnWallHit;
    private float _defaultSpeed;
    private bool _canTurnAround;

    private EnemyRinoAnimationClip _rinoAnimationClip;

    protected override void Awake()
    {
        base.Awake();

        InitValues();
        SetConstraints(RigidbodyConstraints2D.FreezeRotation);
    }

    protected override void InitReferences()
    {
        base.InitReferences();
        _rinoAnimationClip = GetComponentInChildren<EnemyRinoAnimationClip>();
    }

    private void InitValues()
    {
        _canMove = false;
        _defaultSpeed = _moveSpeed;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (_isDead) return;

        HandlePlayerDetection();
        HandleCharge();
        HandleWallHit();
        HandleMovementBlock();
    }

    private void HandlePlayerDetection()
    {
        if (_isPlayerDetected && _isGrounded)
        {
            _canMove = true;
        }
    }

    private void HandleCharge()
    {
        if (!_canMove) return;

        SetVelocityMove();
    }

    private void HandleWallHit()
    {
        if (!_canMove) return;
        if (!_isWallDetected) return;

        _canMove = false;
        _isOnWallHit = true;
        SetVelocityImpact();
        _enemyAnim.SetAnimState("HitWall", true);
        SpeedReset();

        float multiplier = 2;
        float delayTime = _rinoAnimationClip.HitWallClip.length * multiplier;
        Invoke(nameof(ResetOnWallHit), delayTime);
    }

    private void ResetOnWallHit()
    {
        _isOnWallHit = false;
    }

    private void SpeedReset()
    {
        _moveSpeed = _defaultSpeed;
    }

    private void HandleMovementBlock()
    {
        if (!_canMove) return;
        if (_isOnWallHit) return;

        if (!_isGroundInFrontDetected)
        {
            SpeedReset();
            _canMove = false;
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

        HandleSpeedUp();
        HandleTurnAround();
        HandleFlip(_playerTransform.position);
    }

    private void HandleSpeedUp()
    {
        if (!_canMove) return;

        float speedIncrease = Time.deltaTime * _speedUpRate;
        _moveSpeed += speedIncrease;

        if (_moveSpeed >= _maxSpeed)
        {
            _maxSpeed = _moveSpeed;
        }
    }

    private void HandleTurnAround()
    {
        if (!_canTurnAround) return;
        _canTurnAround = false;

        Flip();
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
        if (_isWallDetected && (_isOnWallHit || !_canMove)) return;

        if (ShouldFlipBasedOnPlayer(xVector))
        {
            Flip();
        }
    }

    public void HandleChargeIsOver()
    {
        _enemyAnim.SetAnimState("HitWall", false);
        float delayTime = _rinoAnimationClip.HitWallClip.length;
        Invoke(nameof(Flip), delayTime);
    }

    private void SetVelocityImpact()
    {
        SetVelocity(_impactXPower * -_facingDirection, _impactYPower);
    }

    public override void Death()
    {
        SetConstraints(RigidbodyConstraints2D.None);
        base.Death();
    }
}
