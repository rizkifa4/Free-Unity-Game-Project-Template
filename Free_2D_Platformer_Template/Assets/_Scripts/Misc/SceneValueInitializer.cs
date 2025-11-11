using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneValueInitializer : MonoBehaviour
{
    private Player _player;
    [Header("Respawn Info")]
    private StartPoint _startPoint;

    [Header("Fruits Info")]
    private Fruit[] _fruits;

    [Header("Level Info")]
    [SerializeField] private LevelTimerData _levelTimerData;
    private float _levelTimer;
    private int _levelIndex;
    private int _timerIndex;

    private void Awake()
    {
        InitReferences();
        InitSceneAndTimer();
    }

    private void InitSceneAndTimer()
    {
        int buildIndex = SceneManager.GetActiveScene().buildIndex;
        _levelIndex = buildIndex;
        _timerIndex = GetTimerIndex(buildIndex, _levelTimerData.LevelCount);

        if (_timerIndex < 0)
        {
            _levelTimer = 0f;
            return;
        }

        _levelTimer = _levelTimerData.GetTimer(_timerIndex);
    }

    private void InitReferences()
    {
        _startPoint = FindAnyObjectByType<StartPoint>();
        _player = FindAnyObjectByType<Player>();

        InitFruitsReference();
    }

    private void InitFruitsReference()
    {
        _fruits = FindObjectsByType<Fruit>(FindObjectsSortMode.None);
    }

    private void Start()
    {
        SetGameManagerInitializeSceneValues();
    }

    private void SetGameManagerInitializeSceneValues()
    {
        GameManager gameManager = GameManager.Instance;
        Transform startSpawnPoint = _startPoint.transform.GetChild(0);

        gameManager.SetInitializeSceneValues(
            startSpawnPoint,
            _player,
            _fruits.Length,
            _levelIndex,
            _levelTimer
        );
    }

    private int GetTimerIndex(int buildIndex, int totalLevelCount)
    {
        if (buildIndex < 1 || buildIndex > totalLevelCount)
            return -1;

        return buildIndex - 1;
    }
}
