using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Player")]
    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private float _respawnDelay;
    [SerializeField] private Transform _respawnPoint;
    private Player _player;
    public Player Player => _player;

    [Header("Fruits Info")]
    private int _fruitCollected;
    private int _totalFruits;
    private bool _isFruitsRandomLook;
    public int FruitCollected => _fruitCollected;
    public bool IsFruitRandomLook => _isFruitsRandomLook;

    [Header("Obstacle")]
    [SerializeField] private GameObject _pushArrowPrefab;
    [SerializeField] private GameObject _fallingPlatform;
    public GameObject PushArrowPrefab => _pushArrowPrefab;
    public GameObject FallingPlatform => _fallingPlatform;

    [Header("Game Level Info")]
    [SerializeField] private float _gameTimer;
    [SerializeField] private float _levelStartCooldown;
    [SerializeField] private float _levelTimer;
    [SerializeField] private int _currentLevelIndex;
    private int _nextLevelIndex;
    private float _cooldownTimer;
    private bool _isCooldownRunning;
    private bool _isLevelRunning;

    private UIGameCanvas _uIGameCanvas;

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

    public void SetInitializeSceneValues(Transform startRespawnPoint, Player player, int totalFruits, int currentLevelIndex, float levelTimer)
    {
        _respawnPoint = startRespawnPoint;
        _player = player;
        _totalFruits = totalFruits;
        _fruitCollected = 0;

        _currentLevelIndex = currentLevelIndex;
        _nextLevelIndex = _currentLevelIndex + 1;

        _uIGameCanvas = UIGameCanvas.Instance;
        UpdateFruitGameInfo();

        _levelTimer = levelTimer;
        _gameTimer = 0f;
        _cooldownTimer = _levelStartCooldown;
        _isCooldownRunning = true;
        _isLevelRunning = false;
    }

    private void Update()
    {
        _gameTimer += Time.deltaTime;
        UpdateTimerGameInfo();
    }

    private void UpdateTimerGameInfo()
    {
        if (!_uIGameCanvas) return;
        _uIGameCanvas.UpdateTimerGameInfo(_gameTimer);
    }

    private void UpdateFruitGameInfo()
    {
        if (!_uIGameCanvas) return;
        _uIGameCanvas.UpdateFruitGameInfo(_fruitCollected, _totalFruits);
    }

    public void UpdateRespawnPosition(Transform newRespawnPoint)
    {
        _respawnPoint = newRespawnPoint;
    }

    /// <summary>
    /// Handles player respawn logic with optional difficulty-based restrictions.
    /// </summary>
    /// <remarks>
    /// This method supports scenes that do not include a <see cref="DifficultyManager"/>.  
    /// <para>
    /// - If a <see cref="DifficultyManager"/> exists and the difficulty is set to
    ///   <see cref="DifficultyType.Hard"/>, the respawn is blocked.  
    /// - If the manager does not exist (e.g., prototype or test scenes), respawn is always allowed.  
    /// </para>
    /// This ensures consistent behavior during testing while enforcing difficulty rules in normal gameplay.
    /// </remarks>
    public void RespawnPlayer()
    {
        // Attempt to get the DifficultyManager instance.
        // Note: Not all scenes include this component.
        DifficultyManager difficultyManager = DifficultyManager.Instance;

        if (difficultyManager != null)
        {
            bool isDifficultyHatd = difficultyManager.DifficultyType == DifficultyType.Hard;
            if (isDifficultyHatd) return;
            StartCoroutine(RespawnCoroutine());
            return;
        }

        // No DifficultyManager in this scene (common in testing or prototype scenes) --> always allow respawn.
        StartCoroutine(RespawnCoroutine());
    }

    private IEnumerator RespawnCoroutine()
    {
        yield return new WaitForSeconds(_respawnDelay);
        GameObject newPlayer = Instantiate(_playerPrefab, _respawnPoint.position, Quaternion.identity);
        _player = newPlayer.GetComponent<Player>();
    }

    public void CreateObject(GameObject prefab, Vector3 targetPosition, float delay = 0)
    {
        StartCoroutine(CreateObjectCoroutine(prefab, targetPosition, delay));
    }

    private IEnumerator CreateObjectCoroutine(GameObject prefab, Vector3 target, float delay)
    {
        Vector3 spawnPos = target;
        yield return new WaitForSeconds(delay);
        Instantiate(prefab, spawnPos, Quaternion.identity);
    }

    public void AddFruit(int amount)
    {
        _fruitCollected += amount;
        UpdateFruitGameInfo();
    }

    public void RemoveFruit(int amount = 1)
    {
        _fruitCollected -= amount;
        UpdateFruitGameInfo();
    }

    public void FinishLevel()
    {
        LoadNextScene();
    }

    private void LoadCurrentLevelScene() => SceneManager.LoadScene(_currentLevelIndex);
    private void LoadNextLevelScene() => SceneManager.LoadScene(_nextLevelIndex);
    private void LoadEndScene() => SceneManager.LoadScene(SceneManager.sceneCountInBuildSettings - 1);
    private void LoadNextScene()
    {
        if (!_uIGameCanvas) return;
        UIFadeEffect fadeEffect = _uIGameCanvas.FadeEffect;

        fadeEffect.ScreenFade(1.5f, 1, NoMoreLevel() ? LoadEndScene : LoadNextLevelScene);
    }

    private bool NoMoreLevel()
    {
        int lastLevelIndex = SceneManager.sceneCountInBuildSettings - 2;
        bool noLevel = _currentLevelIndex == lastLevelIndex;
        return noLevel;
    }

    public void RestartLevel()
    {
        UIGameCanvas.Instance.FadeEffect.ScreenFade(.75f, 1, LoadCurrentLevelScene);
    }
}
