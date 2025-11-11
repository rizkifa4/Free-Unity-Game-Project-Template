using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class DeadZone : MonoBehaviour
{
    private GameManager _gameManager;

    private void Start()
    {
        _gameManager = GameManager.Instance;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out Player player))
        {
            player.Damage();
            player.Dead();
            _gameManager.RespawnPlayer();
        }

        if (other.TryGetComponent(out Enemy enemy))
        {
            enemy.Death();
        }
    }
}
