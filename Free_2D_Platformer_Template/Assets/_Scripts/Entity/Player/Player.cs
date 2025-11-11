using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(CapsuleCollider2D))]
public class Player : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField, Range(0f, 10f)] private float _speed = 6f;
    [SerializeField, Range(0f, 20f)] private float _jumpForce = 19f;
    [SerializeField, Range(0f, 15f)] private float _doubleJumpForce = 13f;
    [SerializeField] private float _wallJumpDuration = .5f;

    [Header("Wall Jump")]
    [SerializeField, Range(0f, 10f)] private float _wallJumpXForce = 5f;
    [SerializeField, Range(10f, 20f)] private float _wallJumpYForce = 18f;
    private float _defaultGravityScale;

    [Header("Knockback")]
    [SerializeField, Range(0, 10f)] private float _knockbackXForce = 5f;
    [SerializeField, Range(0, 10f)] private float _knockbackYForce = 8f;
    private float _knockbackDuration;

    [Header("Buffer Jump")]
    [SerializeField] private float _bufferJumpWindow;
    private float _bufferJumpPressed = -1;

    [Header("Coyote Jump")]
    [SerializeField] private float _coyoteJumpWindow = .5f;
    private float _coyoteJumpActivated = -1;


    [Header("Collision")]
    [SerializeField, Range(0f, 2f)] private float _wallCheckDistance = .7f;
    [SerializeField, Range(0f, 2f)] private float _groundCheckDistance = .8f;
    [SerializeField] private LayerMask _whatIsGround;
    [Space]
    [SerializeField] private Transform _enemyCheck;
    [SerializeField] private float _enemyCheckRadius = .2f;
    [SerializeField] private LayerMask _whatIsEnemy;

    [Header("VFX")]
    [SerializeField] private GameObject _deathVFX;

    [Header("State")]
    private bool _isCanControl = false;
    private float _facingDirection = 1;
    private bool _isFacingRight = true;
    private bool _isWallDetected;
    private bool _isJumpPressed;
    private bool _isGrounded;
    private bool _isInAir;
    private bool _canDoubleJump;
    private bool _isWallJump;
    private bool _isKnocked;
    private bool _canTurnAround;

    [Header("Components")]
    private Rigidbody2D _rb;
    private CapsuleCollider2D _playerCollider;
    private PlayerAnimation _playerAnim;

    [Header("Input")]
    private float _xInput;
    private float _yInput;

    private DifficultyType _difficultyType;
    private GameManager _gameManager;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _playerCollider = GetComponent<CapsuleCollider2D>();
        _playerAnim = GetComponentInChildren<PlayerAnimation>();

        _defaultGravityScale = _rb.gravityScale;
    }

    private void Start()
    {
        _gameManager = GameManager.Instance;
        _knockbackDuration = _playerAnim.KnockbackDuration;
        RespawnFinished(false);
        GetDifficulty();
        UpdateSkin();
    }

    private void GetDifficulty()
    {
        DifficultyManager difficultyManager = DifficultyManager.Instance;
        if (!difficultyManager) return;

        _difficultyType = difficultyManager.DifficultyType;
    }

    private void UpdateSkin()
    {
        _playerAnim.UpdatePlayerLook();
    }

    private void FixedUpdate()
    {
        HandleInAirState();

        if (!_isCanControl)
        {
            HandleCollision();
            return;
        }
        if (_isKnocked) return;

        HandleEnemyDetection();
        HandleMove();
        HandleJump();
        HandleWallJump();
        HandleDoubleJump();
        HandleWallSlide();
        HandleCollision();
    }

    /// <summary>
    /// Detects enemies below the player using an overlap circle and
    /// triggers enemy death + bounce response.
    /// </summary>
    private void HandleEnemyDetection()
    {
        bool isMovingUpward = GetVelocityY() > 0f;
        if (isMovingUpward) return;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(_enemyCheck.position, _enemyCheckRadius, _whatIsEnemy);

        foreach (Collider2D enemyCD in colliders)
        {
            if (enemyCD.TryGetComponent(out Enemy enemy))
            {
                enemy.Death();
                SetVelocityJump();
            }
        }
    }

    /// <summary>
    /// Handles horizontal movement using current input while preventing
    /// movement during wall interactions or wall-jump state.
    /// </summary>
    /// <remarks>
    /// Movement is physics-based and executed in <see cref="FixedUpdate"/>.
    /// Disabled when the player is wall-jumping or blocked by a wall.
    /// </remarks>
    private void HandleMove()
    {
        if (_isWallDetected) return;

        if (_isWallJump) return;

        SetVelocityMove();
    }

    private void HandleCollision()
    {
        GroundCollision();
        WallCollision();
    }

    /// <summary>
    /// Performs a downward raycast to determine if the player is grounded.
    /// </summary>
    /// <remarks>
    /// Uses <see cref="_groundCheckDistance"/> and <see cref="_whatIsGround"/>.
    /// </remarks>
    private void GroundCollision()
    {
        Vector2 groundCheckOrigin = transform.position;
        Vector2 groundCheckDirection = Vector2.down;

        RaycastHit2D groundCheckPath = Physics2D.Raycast(
            groundCheckOrigin,
            groundCheckDirection,
            _groundCheckDistance,
            _whatIsGround
        );

        _isGrounded = groundCheckPath.collider != null;
    }

    /// <summary>
    /// Checks for walls in the current facing direction using raycast.
    /// </summary>
    /// <remarks>
    /// Affects wall-jump and wall-slide behavior.
    /// </remarks>
    private void WallCollision()
    {
        Vector2 wallCheckOrigin = transform.position;
        Vector2 wallCheckDirection = Vector2.right * _facingDirection;

        RaycastHit2D wallCheckPath = Physics2D.Raycast(
            wallCheckOrigin,
            wallCheckDirection,
            _wallCheckDistance,
            _whatIsGround
        );

        _isWallDetected = wallCheckPath.collider != null;
    }

    private void HandleInAirState()
    {
        if (_isGrounded && _isInAir)
        {
            ExitInAirState();
        }

        if (!_isGrounded && !_isInAir)
        {
            EnterInAirState();
        }
    }

    private void EnterInAirState()
    {
        _isInAir = true;

        if (GetVelocityY() < 0)
        {
            HandleCoyoteJumpActivation();
        }
    }

    private void ExitInAirState()
    {
        _isInAir = false;
        HandleBufferJump();
    }

    private void HandleBufferJump()
    {
        if (Time.time < _bufferJumpPressed + _bufferJumpWindow)
        {
            _bufferJumpPressed = Time.time - 1;
            HandleJump();
        }
    }

    private void HandleCoyoteJumpActivation()
    {
        _coyoteJumpActivated = Time.time;
    }

    private void HandleCoyoteJumpCancel()
    {
        _coyoteJumpActivated = Time.time - 1;
    }

    /// <summary>
    /// Performs a grounded jump when jump input is pressed.
    /// Resets double-jump availability.
    /// </summary>
    /// <remarks>
    /// Jump is allowed only when grounded or during coyote jump.
    /// </remarks>
    private void HandleJump()
    {
        bool canJump = _isJumpPressed && _isGrounded;
        if (!canJump) return;

        _isJumpPressed = false;
        _canDoubleJump = true;
        SetVelocityJump();
    }

    /// <summary>
    /// Executes double jump if available. Cancels wall-jump coroutine
    /// to ensure clean state transition.
    /// </summary>
    /// <remarks>
    /// Activated only when <see cref="_canDoubleJump"/> is true.
    /// </remarks>
    private void HandleDoubleJump()
    {
        bool canDoubleJump = _isJumpPressed && _canDoubleJump;
        if (!canDoubleJump) return;

        StopCoroutine(WallJumpRoutine());
        _isWallJump = false;

        _isJumpPressed = false;
        _canDoubleJump = false;
        SetVelocityDoubleJump();
    }

    /// <summary>
    /// Handles wall jump mechanics, including velocity assignment,
    /// forced direction flip, and temporary disable of movement control.
    /// </summary>
    /// <remarks>
    /// Starts a timed wall-jump state via <see cref="WallJumpRoutine"/>.
    /// </remarks>
    private void HandleWallJump()
    {
        bool canWallJump = _isJumpPressed && _isWallDetected && !_isGrounded;
        if (!canWallJump) return;

        _isJumpPressed = false;
        _canDoubleJump = true;
        SetVelocityWallJump();
        RequestTurnAround();

        StopAllCoroutines();
        StartCoroutine(WallJumpRoutine());
    }

    private void RequestTurnAround()
    {
        if (_canTurnAround) return;
        _canTurnAround = true;
    }

    private void HandleTurnAround()
    {
        if (!_canTurnAround) return;
        _canTurnAround = false;

        Flip();
    }

    private IEnumerator WallJumpRoutine()
    {
        _isWallJump = true;
        yield return new WaitForSeconds(_wallJumpDuration);
        _isWallJump = false;
    }

    /// <summary>
    /// Enables wall slide when player is falling against a wall.
    /// Reduces downward velocity for controlled descent.
    /// </summary>
    /// <remarks>
    /// Uses vertical input to modify slide strength.
    /// </remarks>
    private void HandleWallSlide()
    {
        bool isFalling = GetVelocityY() < 0f;
        bool canWallSlide = _isWallDetected && isFalling;
        if (!canWallSlide) return;

        float yModifier = _yInput < 0 ? 1 : .05f;
        SetVelocityWallSlide(yModifier);
    }

    /// <summary>
    /// Applies directional knockback force based on source position.
    /// </summary>
    /// <param name="sourceDamagePosition">X position of the damage source.</param>
    /// <remarks>
    /// Disables player control during knockback and plays animation state.
    /// </remarks>
    public void ApplyKnockback(float sourceDamagePosition)
    {
        if (_isKnocked) return;

        float knockbackDirection = 1;

        if (transform.position.x < sourceDamagePosition)
        {
            knockbackDirection = -1;
        }

        SetVelocityKnockback(knockbackDirection);
        StartCoroutine(KnockbackRoutine());
    }

    /// <summary>
    /// Coroutine managing knockback duration.
    /// Restores control after the knockback window ends.
    /// </summary>
    private IEnumerator KnockbackRoutine()
    {
        _isKnocked = true;
        _playerAnim.SetAnimState("IsKnocked", true);
        yield return new WaitForSeconds(_knockbackDuration);
        _isKnocked = false;
        _playerAnim.SetAnimState("IsKnocked", false);
    }

    private void Update()
    {
        if (!_isCanControl)
        {
            HandleAnimation();
            return;
        }
        if (_isKnocked) return;

        HandleMoveInput();
        HandleJumpInput();
        HandleTurnAround();
        HandleFlip();
        HandleAnimation();
    }

    private void HandleAnimation()
    {
        HandleMoveBlendAnimation();
        HandleJumpBlendAnimation();
        HandleWallSlideAnimation();
    }

    private void HandleWallSlideAnimation()
    {
        _playerAnim.SetAnimState("IsWallDetected", _isWallDetected);
    }

    private void HandleMoveBlendAnimation()
    {
        _playerAnim.SetAnimBlend("xVelocity", _rb.linearVelocityX);
    }

    private void HandleJumpBlendAnimation()
    {
        _playerAnim.SetAnimState("IsGrounded", _isGrounded);
        _playerAnim.SetAnimBlend("yVelocity", _rb.linearVelocityY);
    }

    /// <summary>
    /// Captures horizontal and vertical movement input.
    /// </summary>
    /// <remarks>
    /// Processes raw input; movement is applied in <see cref="FixedUpdate"/>.
    /// </remarks>
    private void HandleMoveInput()
    {
        _xInput = Input.GetAxisRaw("Horizontal") * _speed;
        _yInput = Input.GetAxisRaw("Vertical");
    }

    /// <summary>
    /// Processes jump input, including wall jump, coyote jump,
    /// and buffered jump registration.
    /// </summary>
    private void HandleJumpInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            bool isCoyoteJumpAvailable = Time.time < _coyoteJumpActivated + _coyoteJumpWindow;
            RequestJumpInput(isCoyoteJumpAvailable);
            RequestBufferJumpInput();
            HandleCoyoteJumpCancel();
        }
    }

    private void RequestJumpInput(bool isCoyoteJumpAvailable)
    {
        if (_isGrounded || isCoyoteJumpAvailable)
        {
            _isJumpPressed = true;
        }
        else if (_isWallDetected && !_isGrounded)
        {
            _isJumpPressed = true;
        }
        else if (_canDoubleJump)
        {
            _isJumpPressed = true;
        }
    }

    private void RequestBufferJumpInput()
    {
        if (_isInAir)
        {
            _bufferJumpPressed = Time.time;
        }
    }

    /// <summary>
    /// Flips the player's facing direction based on horizontal input.
    /// </summary>
    /// <remarks>
    /// Disabled for one frame when <see cref="_canTurnAround"/> is active
    /// to avoid conflicting with wall-jump forced flip.
    /// </remarks>
    private void HandleFlip()
    {
        if (_canTurnAround) return;

        if (_xInput > 0 && !_isFacingRight || _xInput < 0 && _isFacingRight)
        {
            Flip();
        }
    }

    private void Flip()
    {
        transform.Rotate(0, 180, 0);
        _isFacingRight = !_isFacingRight;
        _facingDirection *= -1;
    }

    /// <summary>
    /// Handles damage logic depending on the current difficulty type.
    /// </summary>
    /// <remarks>
    /// Normal mode: consumes fruit before killing player.  
    /// Hard mode: instant death.
    /// </remarks>
    public void Damage()
    {
        if (_difficultyType == DifficultyType.Normal)
        {
            if (_gameManager.FruitCollected <= 0)
            {
                Dead();
                _gameManager.RestartLevel();
            }
            else
            {
                _gameManager.RemoveFruit();
            }
            return;
        }

        if (_difficultyType == DifficultyType.Hard)
        {
            Dead();
            _gameManager.RestartLevel();
        }
    }

    /// <summary>
    /// Spawns the death visual effect and destroys the player object.
    /// </summary>
    public void Dead()
    {
        GameObject newDeathVFX = Instantiate(_deathVFX, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    /// <summary>
    /// Enables or disables player control and gravity during respawn.
    /// </summary>
    /// <param name="finished">True when respawn is completed.</param>
    public void RespawnFinished(bool finished)
    {
        float zeroGravityScale = 0f;

        _rb.gravityScale = finished ? _defaultGravityScale : zeroGravityScale;
        _isCanControl = finished;
        _playerCollider.enabled = finished;
    }

    public Vector2 GetRBPosition() => _rb.position;
    public Vector2 SetRBPosition(Vector2 newPosition)
    {
        _rb.position = newPosition;
        return _rb.position;
    }

    public float GetVelocityY() => _rb.linearVelocityY;

    private void SetVelocity(float xValue, float yValue)
    {
        _rb.linearVelocity = new(xValue, yValue);
    }

    private void SetVelocityZero()
    {
        _rb.linearVelocity = Vector2.zero;
    }

    private void SetVelocityMove()
    {
        SetVelocity(_xInput, _rb.linearVelocityY);
    }

    private void SetVelocityJump()
    {
        SetVelocity(_rb.linearVelocityX, _jumpForce);
    }

    private void SetVelocityDoubleJump()
    {
        SetVelocity(_rb.linearVelocityX, _doubleJumpForce);
    }

    private void SetVelocityWallJump()
    {
        SetVelocity(_wallJumpXForce * -_facingDirection, _wallJumpYForce);
    }

    private void SetVelocityWallSlide(float yModifier)
    {
        SetVelocity(_rb.linearVelocityX, _rb.linearVelocityY * yModifier);
    }

    private void SetVelocityKnockback(float direction)
    {
        SetVelocity(_knockbackXForce * direction, _knockbackYForce);
    }

    public void SetVelocityRising(float yModifier)
    {
        SetVelocity(_rb.linearVelocityX, yModifier);
    }

    /// <summary>
    /// Applies an impulse force that temporarily disables player control.
    /// </summary>
    /// <param name="direction">Impulse direction.</param>
    /// <param name="pushDuration">Duration before control is restored.</param>
    public void Push(Vector2 direction, float pushDuration = 0)
    {
        StartCoroutine(PushCoroutine(direction, pushDuration));
    }

    private IEnumerator PushCoroutine(Vector2 direction, float pushDuration)
    {
        _isCanControl = false;
        SetVelocityZero();
        _rb.AddForce(direction, ForceMode2D.Impulse);
        yield return new WaitForSeconds(pushDuration);
        _isCanControl = true;
    }

    private void OnDrawGizmos()
    {
        Vector2 startPos = transform.position;

        Vector2 groundDirection = Vector2.down * _groundCheckDistance;
        Debug.DrawRay(startPos, groundDirection, _isGrounded ? Color.green : Color.red);

        Vector2 wallCheckDirection = _facingDirection * _wallCheckDistance * Vector2.right;
        Debug.DrawRay(startPos, wallCheckDirection, _isWallDetected ? Color.green : Color.red);

        Gizmos.DrawWireSphere(_enemyCheck.position, _enemyCheckRadius);
    }
}
