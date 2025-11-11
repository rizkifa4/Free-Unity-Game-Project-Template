using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(Rigidbody2D))]
public class EnemySnailBody : MonoBehaviour
{
    private SpriteRenderer _sr;
    private Rigidbody2D _rb;
    private float _zRotation;

    public void SetupBody(float yVelocity, float zRotation, int facingDirection)
    {
        _sr = GetComponent<SpriteRenderer>();
        _rb = GetComponent<Rigidbody2D>();

        Vector2 velocity = new(_rb.linearVelocityX, yVelocity);
        _rb.linearVelocity = velocity;

        _zRotation = zRotation;

        _sr.flipX = facingDirection == 1;
    }

    private void Update()
    {
        transform.Rotate(0, 0, _zRotation * Time.deltaTime);
    }
}
