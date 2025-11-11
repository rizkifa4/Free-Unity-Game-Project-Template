using UnityEngine;

public class EnemyRinoAnimationEvent : MonoBehaviour
{
    private EnemyRino _enemyRino;

    private void Awake()
    {
        _enemyRino = GetComponentInParent<EnemyRino>();
    }

    private void OnChargeAnimationEnd()
    {
        _enemyRino.HandleChargeIsOver();
    }
}
