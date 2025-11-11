using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Checkpoint : MonoBehaviour
{
    private Animator _anim;
    private bool _isActive;

    private GameManager _gameManager;

    private void Awake()
    {
        _anim = GetComponent<Animator>();
    }

    private void Start()
    {
        _gameManager = GameManager.Instance;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_isActive) return;

        if (other.TryGetComponent<Player>(out _))
        {
            ActivateCheckpoint();
        }
    }

    private void ActivateCheckpoint()
    {
        _isActive = true;
        _anim.SetBool("Active", _isActive);
        _gameManager.UpdateRespawnPosition(transform);
    }
}
