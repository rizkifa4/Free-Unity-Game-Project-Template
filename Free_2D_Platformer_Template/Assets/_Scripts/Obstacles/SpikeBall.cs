using UnityEngine;

public class SpikeBall : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _spikeRb;
    [SerializeField] private float _pushForce;

    private void Awake()
    {
        Vector2 pushVector = new(_pushForce, 0);
        _spikeRb.AddForce(pushVector, ForceMode2D.Impulse);
    }
}
