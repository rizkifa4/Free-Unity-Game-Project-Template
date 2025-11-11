using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider2D), typeof(Animator))]
public class Fire : MonoBehaviour
{
    [SerializeField] private float _offDuration;

    private FireButton _fireButton;
    private bool _isActive;
    private CapsuleCollider2D _collider;
    private Animator _anim;

    private void Awake()
    {
        _collider = GetComponent<CapsuleCollider2D>();
        _anim = GetComponent<Animator>();
        _fireButton = GetComponentInChildren<FireButton>();
    }

    private void Start()
    {
        SetFire(true);
    }

    private void SetFire(bool active)
    {
        _anim.SetBool("Active", active);
        _collider.enabled = active;
        _isActive = active;
    }

    public void SwitchOffFire()
    {
        if (!_isActive) return;

        StartCoroutine(FireCoroutine());
    }

    private IEnumerator FireCoroutine()
    {
        SetFire(false);
        yield return new WaitForSeconds(_offDuration);
        SetFire(true);
    }
}
