using UnityEngine;

[RequireComponent(typeof(Animator))]
public class StartPoint : MonoBehaviour
{
    private Animator _anim;
    private void Awake()
    {
        _anim = GetComponent<Animator>();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent<Player>(out _))
        {
            _anim.SetTrigger("Activate");
        }
    }
}
