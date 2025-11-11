using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIFadeEffect : MonoBehaviour
{
    [SerializeField] private Image _fadeImage;

    public void ScreenFade(float duration, float targetAlpha, Action onComplete = null)
    {
        StartCoroutine(FadeCoroutine(duration, targetAlpha, onComplete));
    }

    private IEnumerator FadeCoroutine(float duration, float targetAlpha, Action onComplete)
    {
        float time = 0f;
        Color currentColor = _fadeImage.color;
        float startAlpa = currentColor.a;

        while (time < duration)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpa, targetAlpha, time / duration);
            _fadeImage.color = new(currentColor.r, currentColor.g, currentColor.b, alpha);
            yield return null;
        }

        _fadeImage.color = new(currentColor.r, currentColor.g, currentColor.b, targetAlpha);
        onComplete?.Invoke();
    }
}