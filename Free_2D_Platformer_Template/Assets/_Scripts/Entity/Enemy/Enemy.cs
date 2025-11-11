using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class Enemy : MonoBehaviour
{
    [Header("Movement Info")]
    [SerializeField, Range(0f, 10f)] protected float _moveSpeed = 3f;
    [SerializeField, Range(0f, 5f)] protected float _idleDuration = 1.5f;
    protected float _idleTimer;
    protected bool _canMove = true;

    [Header("Death Info")]
    [SerializeField, Range(0f, 10f)] protected float _deathImpactSeed = 5f;
    [SerializeField, Range(0f, 360f)] protected float _deathRotationSpeed = 150f;
    protected int _deathRotationDirection = 1;
    protected bool _isDead;

    [Header("Collision Info")]
    [SerializeField, Range(0f, 5f)] protected float _groundCheckDistance;
    [SerializeField, Range(0f, 5f)] protected float _wallCheckDistance;
    [SerializeField, Range(0f, 20f)] protected float _playerCheckDistance;
    [SerializeField] protected LayerMask _playerLayer;
    [SerializeField] protected LayerMask _groundLayer;
    [SerializeField] protected Transform _groundCheck;

    [Header("Facing Direction")]
    [SerializeField] protected int _facingDirection = -1;
    [SerializeField] protected bool _facingRight = false;

    protected bool _isGrounded;
    protected bool _isPlayerDetected;
    protected bool _isGroundInFrontDetected;
    protected bool _isWallDetected;

    [Space]
    protected Collider2D[] _colliders;
    private SpriteRenderer _sr;
    protected Rigidbody2D _rb;
    protected EnemyAnimation _enemyAnim;
    protected Transform _playerTransform;

    protected virtual void Awake()
    {
        InitReferences();
    }

    protected virtual void InitReferences()
    {
        _rb = GetComponent<Rigidbody2D>();
        _sr = GetComponentInChildren<SpriteRenderer>();
        _enemyAnim = GetComponentInChildren<EnemyAnimation>();
        _colliders = GetComponentsInChildren<Collider2D>();
    }

    protected virtual void Start()
    {
        SetFacing();
        SetReferences();
    }

    protected virtual void SetReferences()
    {
        InvokeRepeating(nameof(SetPlayerReference), 0, 1);
    }

    private void SetPlayerReference()
    {
        if (_playerTransform == null)
        {
            _playerTransform = GameManager.Instance.Player.transform;
        }
    }

    protected void SetConstraints(RigidbodyConstraints2D constraints)
    {
        _rb.constraints = constraints;
    }

    protected virtual void SetFacing()
    {
        if (_sr.flipX && !_facingRight)
        {
            _sr.flipX = false;
            Flip();
        }
    }

    protected virtual void FixedUpdate()
    {
        HandleCollision();
    }

    protected virtual void HandleCollision()
    {
        Vector2 origin = transform.position;
        Vector2 groundDirection = Vector2.down;
        Vector2 facingDirection = Vector2.right * _facingDirection;

        _isGrounded = Physics2D.Raycast(origin, groundDirection, _groundCheckDistance, _groundLayer);
        _isGroundInFrontDetected = Physics2D.Raycast(_groundCheck.position, groundDirection, _groundCheckDistance, _groundLayer);
        _isWallDetected = Physics2D.Raycast(origin, facingDirection, _wallCheckDistance, _groundLayer);
        _isPlayerDetected = Physics2D.Raycast(origin, facingDirection, _playerCheckDistance, _playerLayer);
    }

    protected virtual void Update()
    {
        HandleIdleMoveAnimation();

        _idleTimer -= Time.deltaTime;

        HandleDeathRotation();
    }

    private void HandleDeathRotation()
    {
        if (!_isDead) return;

        float deltaSpeed = _deathRotationSpeed * Time.deltaTime;
        float rotationAngle = _deathRotationDirection * deltaSpeed;
        transform.Rotate(0, 0, rotationAngle);
    }

    public virtual void Death()
    {
        foreach (Collider2D collider in _colliders)
        {
            collider.enabled = false;
        }

        _isDead = true;

        _enemyAnim.SetAnimTrigger("Hit");

        SetVelocityDeathImpact();

        bool canRotation = Random.Range(0f, 100f) < 50f;
        if (canRotation) _deathRotationDirection *= -1;

        float delayTime = 10f;
        Destroy(gameObject, delayTime);
    }

    protected virtual void HandleIdleMoveAnimation()
    {
        _enemyAnim.SetAnimBlend("xVelocity", _rb.linearVelocityX);
    }

    protected virtual void HandleFlip(Vector2 xVector)
    {
        if (xVector.x < transform.position.x && _facingRight || xVector.x > transform.position.x & !_facingRight)
        {
            Flip();
        }
    }

    protected virtual void Flip()
    {
        _facingDirection *= -1;
        transform.Rotate(0, 180, 0);
        _facingRight = !_facingRight;
    }

    protected void SetVelocity(float xValue, float yValue)
    {
        _rb.linearVelocity = new(xValue, yValue);
    }

    protected void SetVelocityZero()
    {
        _rb.linearVelocity = Vector2.zero;
    }

    protected void SetVelocityMove()
    {
        SetVelocity(_moveSpeed * _facingDirection, _rb.linearVelocityY);
    }

    protected void SetVelocityDeathImpact()
    {
        SetVelocity(_rb.linearVelocityX, _deathImpactSeed);
    }

    protected virtual void OnDrawGizmos()
    {
        Gizmos.color = _isGrounded ? Color.green : Color.red;
        Gizmos.DrawRay(transform.position, Vector2.down * _groundCheckDistance);

        Gizmos.color = _isGroundInFrontDetected ? Color.green : Color.red;
        if (_groundCheck != null)
        {
            Gizmos.DrawRay(_groundCheck.position, Vector2.down * _groundCheckDistance);
        }

        Gizmos.color = _isWallDetected ? Color.red : Color.green;
        Gizmos.DrawRay(transform.position, _facingDirection * _wallCheckDistance * Vector2.right);

        Gizmos.color = _isPlayerDetected ? Color.red : Color.green;
        Gizmos.DrawRay(transform.position, _facingDirection * _playerCheckDistance * Vector2.right);
    }
}
