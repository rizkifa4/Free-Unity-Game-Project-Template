using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIMainMenu : MonoBehaviour
{
    [SerializeField] private GameObject[] _uiElements;

    [SerializeField] private string _firstLevelName;
    [SerializeField] private string _creditsSceneName;

    private UIFadeEffect _fadeEffect;

    private void Awake()
    {
        _fadeEffect = GetComponentInChildren<UIFadeEffect>();
    }

    private void Start()
    {
        _fadeEffect.ScreenFade(1.5f, 0f);
    }

    public void SwitchUI(GameObject uiToEnable)
    {
        foreach (GameObject ui in _uiElements)
        {
            ui.SetActive(false);
        }

        uiToEnable.SetActive(true);
    }

    public void NewGame()
    {
        _fadeEffect.ScreenFade(1.5f, 1f, LoadLevelScene);
    }

    public void LoadLevelScene()
    {
        SceneManager.LoadScene(1);
    }

    public void LoadCreditsScene()
    {
        SceneManager.LoadScene(_creditsSceneName);
    }
}