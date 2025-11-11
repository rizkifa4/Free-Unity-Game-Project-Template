using UnityEngine;

public class EnemySnailAnimationEvent : MonoBehaviour
{
    private EnemySnail _enemySnail;

    private void Awake()
    {
        _enemySnail = GetComponentInParent<EnemySnail>();
    }

    private void OnHitAnimationEnd()
    {
        _enemySnail.HandleCreateBody();
    }
}
