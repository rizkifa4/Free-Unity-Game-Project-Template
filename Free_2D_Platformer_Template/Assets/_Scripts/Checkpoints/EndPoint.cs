using UnityEngine;

[RequireComponent(typeof(Animator))]
public class EndPoint : MonoBehaviour
{
    private Animator _anim;
    private bool _isTrigger;

    private void Awake()
    {
        _anim = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_isTrigger) return;

        if (other.TryGetComponent<Player>(out _))
        {
            _isTrigger = true;
            _anim.SetTrigger("Activate");
            GameManager.Instance.FinishLevel();
        }
    }
}
