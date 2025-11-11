using UnityEngine;

[RequireComponent(typeof(BoxCollider2D), typeof(Animator))]
public class FireButton : MonoBehaviour
{
    private Animator _anim;
    private Fire _fireObstacle;

    private void Awake()
    {
        _anim = GetComponent<Animator>();
        _fireObstacle = GetComponentInParent<Fire>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<Player>(out _))
        {
            _anim.SetTrigger("Activated");
            _fireObstacle.SwitchOffFire();
        }
    }
}
