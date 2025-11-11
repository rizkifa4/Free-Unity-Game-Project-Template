using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Trampoline : MonoBehaviour
{
    [SerializeField, Range(0f, 30f)] private float _pushForce = 25f;
    [SerializeField, Range(0f, 1f)] private float _pushDuration = .5f;

    protected Animator _anim;

    protected virtual void Awake()
    {
        _anim = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out Player player))
        {
            Push(player);
        }
    }

    protected virtual void Push(Player player)
    {
        _anim.SetTrigger("Activate");
        player.Push(transform.up * _pushForce, _pushDuration);
    }
}
