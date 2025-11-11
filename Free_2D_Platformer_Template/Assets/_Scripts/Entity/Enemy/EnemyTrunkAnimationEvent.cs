using UnityEngine;

public class EnemyTrunkAnimationEvent : MonoBehaviour
{
    private EnemyTrunk _enemyTrunk;

    private void Awake()
    {
        _enemyTrunk = GetComponentInParent<EnemyTrunk>();
    }

    private void EventHandleShootBullet()
    {
        _enemyTrunk.HandleShootBullet();
    }
}