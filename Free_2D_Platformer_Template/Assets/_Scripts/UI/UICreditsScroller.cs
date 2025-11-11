using UnityEngine;
using UnityEngine.SceneManagement;

public class UICreditsScroller : MonoBehaviour
{
    [SerializeField] private RectTransform _creditsRectTransform;
    [SerializeField, Range(1f, 200f)] private float _scrollSpeed = 100f;
    [SerializeField] private float _offScreenPosition = 1300f;

    [SerializeField] private string _mainMeniSceneName = "MainMenu";
    [SerializeField] private bool _isGame;
    [SerializeField] private bool _isDone;
    private bool _isSkipCreddits;

    private UIFadeEffect _fadeEffect;

    private void Awake()
    {
        _fadeEffect = GetComponentInChildren<UIFadeEffect>();
        _fadeEffect.ScreenFade(2f, 0f);
    }

    private void Update()
    {
        if (_isDone) return;

        float deltaSpeed = _scrollSpeed * Time.deltaTime;
        Vector2 moveDelta = new(0f, deltaSpeed);
        _creditsRectTransform.anchoredPosition += moveDelta;

        if (_creditsRectTransform.anchoredPosition.y > _offScreenPosition)
        {
            _isDone = true;
            GoToMainMenu();
        }
    }

    public void SkipCredits()
    {
        if (_isSkipCreddits)
        {
            GoToMainMenu();
            return;
        }

        _scrollSpeed *= 10;
        _isSkipCreddits = true;
    }

    private void GoToMainMenu()
    {
        if (!_isGame) return;
        _fadeEffect.ScreenFade(1f, 1f, SwitchToMenuScene);
    }

    private void SwitchToMenuScene()
    {
        SceneManager.LoadScene(_mainMeniSceneName);
    }

    public void OpenUrl(string url)
    {
        Application.OpenURL(url);
    }
}