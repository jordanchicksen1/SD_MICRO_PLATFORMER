using System.Collections;
using UnityEngine;

public class ScreenFader : MonoBehaviour
{
    [SerializeField] CanvasGroup group;
    [SerializeField] float defaultFadeDuration = 0.35f;

    [Header("Scene Start Fade")]
    [SerializeField] bool fadeFromBlackOnStart = true;
    [SerializeField] float fadeInDuration = 0.4f;

    void Awake()
    {
        if (!group) group = GetComponent<CanvasGroup>();

        if (!group) return;

        group.blocksRaycasts = false;
        group.interactable = false;

        if (fadeFromBlackOnStart)
            group.alpha = 1f;   // start black
        else
            group.alpha = 0f;
    }

    void Start()
    {
        if (fadeFromBlackOnStart)
            StartCoroutine(FadeInRoutine());
    }

    IEnumerator FadeInRoutine()
    {
        yield return null; // wait one frame so scene fully renders

        yield return FadeRoutine(0f, fadeInDuration);
    }

    public Coroutine FadeTo(float targetAlpha, float duration = -1f)
    {
        if (duration < 0f) duration = defaultFadeDuration;
        return StartCoroutine(FadeRoutine(targetAlpha, duration));
    }

    IEnumerator FadeRoutine(float targetAlpha, float duration)
    {
        if (!group) yield break;

        float start = group.alpha;
        float t = 0f;

        bool becomingOpaque = targetAlpha > start;
        group.blocksRaycasts = becomingOpaque;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime; // IMPORTANT
            float a = Mathf.SmoothStep(0f, 1f, t / duration);

            group.alpha = Mathf.Lerp(start, targetAlpha, a);
            yield return null;
        }

        group.alpha = targetAlpha;

        if (Mathf.Approximately(targetAlpha, 0f))
            group.blocksRaycasts = false;
    }
}
