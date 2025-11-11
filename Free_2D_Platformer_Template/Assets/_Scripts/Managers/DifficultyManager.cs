using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    [field: SerializeField] public DifficultyType DifficultyType { get; private set; }

    public static DifficultyManager Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetDifficulty(DifficultyType newDifficulty) => DifficultyType = newDifficulty;
}

public enum DifficultyType { Easy = 1, Normal, Hard, }