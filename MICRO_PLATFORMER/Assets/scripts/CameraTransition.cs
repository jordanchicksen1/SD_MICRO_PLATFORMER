using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CameraTransition : MonoBehaviour
{
    [SerializeField] Image overlay;

    [Header("Timing")]
    [SerializeField] float fadeOutTime = 0.12f;
    [SerializeField] float fadeInTime = 0.18f;

    Coroutine transitionRoutine;
    public bool IsTransitioning
    {
        get { return transitionRoutine != null; }
    }

    void Awake()
    {
        if (overlay != null)
        {
            Color color = overlay.color;
            color.a = 0f;
            overlay.color = color;
        }
    }

    public void PlayTransition(System.Action switchCamera)
    {
        if (transitionRoutine != null)
            StopCoroutine(transitionRoutine);

        transitionRoutine =
            StartCoroutine(TransitionRoutine(switchCamera));
    }

    IEnumerator TransitionRoutine(System.Action switchCamera)
    {
        // Fade overlay IN
        yield return Fade(0f, 1f, fadeOutTime);

        // Switch cameras while the screen is covered.
        switchCamera?.Invoke();

        // Fade overlay OUT
        yield return Fade(1f, 0f, fadeInTime);

        transitionRoutine = null;
    }

    IEnumerator Fade(float startAlpha, float endAlpha, float duration)
    {
        float t = 0f;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;

            float progress =
                Mathf.Clamp01(t / duration);

            Color color = overlay.color;
            color.a =
                Mathf.Lerp(
                    startAlpha,
                    endAlpha,
                    progress
                );

            overlay.color = color;

            yield return null;
        }

        Color finalColor = overlay.color;
        finalColor.a = endAlpha;
        overlay.color = finalColor;
    }
}