using UnityEngine;

public class EnemyPlantAnimationEvent : MonoBehaviour
{
    private EnemyPlant _enemyPlant;

    private void Awake()
    {
        _enemyPlant = GetComponentInParent<EnemyPlant>();
    }

    private void EventHandleShootBullet()
    {
        _enemyPlant.HandleShootBullet();
    }
}