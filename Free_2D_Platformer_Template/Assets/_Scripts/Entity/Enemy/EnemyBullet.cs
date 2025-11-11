using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(Rigidbody2D), typeof(CircleCollider2D))]
public class EnemyBullet : MonoBehaviour
{
    [SerializeField] private LayerMask _playerLayer = 1 << 6;
    [SerializeField] private LayerMask _groundLayer = 1 << 3;
    private SpriteRenderer _sr;
    private Rigidbody2D _rb;

    private void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
        _rb = GetComponent<Rigidbody2D>();
    }

    public void SpriteFlip() => _sr.flipX = !_sr.flipX;

    public void SetVelocity(Vector2 velocity) => _rb.linearVelocity = velocity;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (1 << other.gameObject.layer == _playerLayer && other.TryGetComponent(out Player player))
        {
            player.ApplyKnockback(transform.position.x);
            Destroy(gameObject);
        }

        if (1 << other.gameObject.layer == _groundLayer)
        {
            Destroy(gameObject);
        }
    }
}
