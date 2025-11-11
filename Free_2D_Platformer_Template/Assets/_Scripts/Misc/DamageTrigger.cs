using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class DamageTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out Player player))
        {
            player.Damage();
            player.ApplyKnockback(transform.position.x);
        }
    }
}