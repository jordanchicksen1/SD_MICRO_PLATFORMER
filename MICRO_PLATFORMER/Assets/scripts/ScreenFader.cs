using System.Collections;
using UnityEngine;

public class ScreenFader : MonoBehaviour
{
    [SerializeField] CanvasGroup group;
    [SerializeField] float defaultFadeDuration = 0.35f;

    void Awake()
    {
        if (!group) group = GetComponent<CanvasGroup>();
        if (group)
        {
            group.alpha = 0f;
            group.blocksRaycasts = false;
            group.interactable = false;
        }
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

        // block clicks while fading in
        bool becomingOpaque = targetAlpha > start;
        group.blocksRaycasts = becomingOpaque;

        while (t < duration)
        {
            t += Time.deltaTime;
            float a = Mathf.Clamp01(t / Mathf.Max(0.0001f, duration));
            group.alpha = Mathf.Lerp(start, targetAlpha, a);
            yield return null;
        }

        group.alpha = targetAlpha;

        // if fully faded out, unblock clicks again
        if (Mathf.Approximately(targetAlpha, 0f))
            group.blocksRaycasts = false;
    }
}
