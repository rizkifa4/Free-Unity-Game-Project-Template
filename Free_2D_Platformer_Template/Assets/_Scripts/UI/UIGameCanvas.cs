using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIGameCanvas : MonoBehaviour
{
    public static UIGameCanvas Instance;

    [SerializeField] private TextMeshProUGUI _fruitCountText;
    [SerializeField] private TextMeshProUGUI _timerText;
    [SerializeField] private GameObject _pauseCanvas;
    private bool _isPause;

    public UIFadeEffect FadeEffect { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        FadeEffect = GetComponentInChildren<UIFadeEffect>();
    }

    public void Start()
    {
        FadeEffect.ScreenFade(1, 0);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            PauseButton();
        }
    }

    public void PauseButton()
    {
        if (_isPause)
        {
            _isPause = false;
            Time.timeScale = 1;
            _pauseCanvas.SetActive(false);
        }
        else
        {
            _isPause = true;
            Time.timeScale = 0;
            _pauseCanvas.SetActive(true);
        }
    }

    public void GoToMainMenuButton()
    {
        SceneManager.LoadScene(0);
    }

    public void UpdateFruitGameInfo(int collected, int totalFruits)
    {
        _fruitCountText.text = $"{collected} / {totalFruits}";
    }

    public void UpdateTimerGameInfo(float time)
    {
        _timerText.text = $"{time:F2}s";
    }
}