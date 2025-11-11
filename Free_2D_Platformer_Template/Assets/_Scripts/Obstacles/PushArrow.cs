using UnityEngine;

public class PushArrow : Trampoline
{
    [Header("Additional Info")]
    [SerializeField] private float _rotationSpeed = 120f;
    [SerializeField] private float _cooldown = 2f;
    [SerializeField] private bool _isRightRotation;
    private int _direction = -1;

    [Space]
    [SerializeField] private float _scaleUpSpeed = 10f;
    [SerializeField] private Vector3 _targetScale;

    private GameManager _gameManager;
    private bool _isTriggerPush = false;
    private bool _isTriggerDestroy = false;

    protected override void Awake()
    {
        base.Awake();
        transform.localScale = new(.25f, .25f, .25f);
    }

    private void Start()
    {
        _gameManager = GameManager.Instance;
    }

    private void Update()
    {
        HandleScaleUp();
        HandleRotation();
    }

    private void HandleScaleUp()
    {
        if (transform.localScale.x < _targetScale.x)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, _targetScale, _scaleUpSpeed * Time.deltaTime);
        }
    }

    private void HandleRotation()
    {
        _direction = _isRightRotation ? -1 : 1;
        float deltaSpeed = _rotationSpeed * Time.deltaTime;
        float rotationAngle = _direction * deltaSpeed;
        transform.Rotate(0, 0, rotationAngle);
    }

    protected override void Push(Player player)
    {
        if (_isTriggerPush) return;
        _isTriggerPush = true;

        base.Push(player);
    }

    /// <summary>
    /// Called at the end of the Push animation event.  
    /// Spawns a new <c>PushArrow</c> and destroys this object.  
    /// </summary>
    /// <remarks>
    /// This method ensures it runs only once by checking <see cref="_isTriggerDestroy"/>.  
    /// <para>
    /// Normally, the object is destroyed immediately after spawning a new one.  
    /// During debugging, you can use a custom <c>Vector3</c> position to confirm  
    /// the spawned <c>PushArrow</c> appears at a different location.  
    /// </para>
    /// <para>
    /// The optional 2-second delay can help verify that the method is called only once,  
    /// even when the animation loops faster than the cooldown duration.  
    /// </para>
    /// </remarks>
    private void DestroyItSelf()
    {
        if (_isTriggerDestroy) return;
        _isTriggerDestroy = true;

        GameObject newPushArrow = _gameManager.PushArrowPrefab;

        // For debugging only: spawn at a different position to visualize the spawn location.
        // Vector3 newPos = new(5, 0);
        // _gameManager.CreateObject(newPushArrow, newPos, _cooldown);

        _gameManager.CreateObject(newPushArrow, transform.position, _cooldown);
        Destroy(gameObject);

        // For debugging only: add delay to confirm this method isn't repeatedly called.
        // Destroy(gameObject, 2);
    }
}
