using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class FallingPlatform : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private float _travelDistance;
    [SerializeField] private BoxCollider2D[] _boxColliders;

    [Header("Platform fall details")]
    [SerializeField] private float _impactSpeed;
    [SerializeField] private float _impactDuration;
    [SerializeField] private float _fallDelay;
    private float _impactTimer;
    private bool _impactActive;

    private Vector3[] _waypoints;
    private int _waypointsIndex;
    private bool _canMove;
    private Rigidbody2D _rb;
    private Animator _anim;
    private Vector3 _spawnPosition;
    private bool _isTriggerDestroy;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();

        _spawnPosition = transform.position;
        Debug.Log(transform.position);
    }

    private IEnumerator Start()
    {
        SetupWaypoints();
        float randomDelay = Random.Range(0f, .5f);
        yield return new WaitForSeconds(randomDelay);
        _canMove = true;
    }

    private void SetupWaypoints()
    {
        _waypoints = new Vector3[2];

        float yOffset = _travelDistance / 2;
        _waypoints[0] = transform.position + new Vector3(0, yOffset, 0);
        _waypoints[1] = transform.position + new Vector3(0, -yOffset, 0);
    }

    private void Update()
    {
        HandleMovement();
        HandleImpact();
    }

    private void HandleMovement()
    {
        if (!_canMove) return;

        transform.position = Vector2.MoveTowards(transform.position, _waypoints[_waypointsIndex], _speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, _waypoints[_waypointsIndex]) < .1f)
        {
            _waypointsIndex++;

            if (_waypointsIndex >= _waypoints.Length)
            {
                _waypointsIndex = 0;
            }
        }
    }

    private void HandleImpact()
    {
        if (_impactTimer < 0) return;

        _impactTimer -= Time.deltaTime;

        transform.position = Vector2.MoveTowards(transform.position, transform.position + (Vector3.down * 10), _impactSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<Player>(out _) && !_impactActive)
        {
            Invoke(nameof(SwitchOffPlatform), _fallDelay);
            _impactTimer = _impactDuration;
            _impactActive = true;
        }

        if (other.TryGetComponent<DestroyTrigger>(out _) && _impactActive)
        {
            SelfDestroy();
        }
    }

    private void SwitchOffPlatform()
    {
        _anim.SetTrigger("Deactivate");
        _canMove = false;

        _rb.bodyType = RigidbodyType2D.Dynamic;
        _rb.gravityScale = 3.5f;
        _rb.linearDamping = .5f;

        foreach (BoxCollider2D bc in _boxColliders)
        {
            bc.enabled = false;
        }
    }

    private void SelfDestroy()
    {
        if (_isTriggerDestroy) return;
        _isTriggerDestroy = true;
        Debug.Log("Self Destroy is called");
        Debug.Log(_isTriggerDestroy);

        float delay = 5f;
        GameManager gameManager = GameManager.Instance;
        gameManager.CreateObject(gameManager.FallingPlatform, _spawnPosition, delay);
        Destroy(gameObject, delay);
    }
}
