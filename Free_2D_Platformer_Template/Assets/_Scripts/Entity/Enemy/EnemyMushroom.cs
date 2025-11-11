using UnityEngine;

public class EnemyMushroom : Enemy
{
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

        HandleTurnAround();
    }

    private void HandleTurnAround()
    {
        if (!_canTurnAround) return;
        _canTurnAround = false;

        Flip();
        _idleTimer = _idleDuration;
    }
}
