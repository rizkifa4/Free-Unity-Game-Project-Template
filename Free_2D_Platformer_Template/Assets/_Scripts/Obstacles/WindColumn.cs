using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class WindColumn : MonoBehaviour
{
    [SerializeField, Range(15f, 25f)] private float _riseSpeed = 15f;
    [SerializeField, Range(-1f, 1f)] private float _holdPadding = -.5f;
    private BoxCollider2D _cd;

    private void Awake()
    {
        _cd = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.TryGetComponent(out Player player)) return;

        float yValue = Mathf.Max(0f, player.GetVelocityY());
        player.SetVelocityRising(yValue);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.TryGetComponent(out Player player)) return;

        float topY = _cd.bounds.max.y - _holdPadding;
        Vector2 rbPosition = player.GetRBPosition();

        if (rbPosition.y < topY)
        {
            player.SetVelocityRising(_riseSpeed);
            return;
        }

        Vector2 pos = new(rbPosition.x, topY);
        player.SetRBPosition(pos);
        player.SetVelocityRising(0f);
    }
}