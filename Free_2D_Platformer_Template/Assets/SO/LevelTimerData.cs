using UnityEngine;

[CreateAssetMenu(fileName = "LevelTimerData", menuName = "Gameplay Data/LevelTimerData")]
public class LevelTimerData : ScriptableObject
{
    [SerializeField] private float[] _levelTimers;
    public float[] LevelTimers => _levelTimers;
    public int LevelCount => _levelTimers.Length;

    public float GetTimer(int index)
    {
        if (index < 0 || index >= _levelTimers.Length)
        {
            return 0f;
        }

        return _levelTimers[index];
    }
}
