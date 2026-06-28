using System.Collections;
using UnityEngine;

public class UIPanelSlide : MonoBehaviour
{
    [SerializeField] RectTransform panel;

    [Header("Animation")]
    [SerializeField] float duration = 0.35f;

    [SerializeField] Vector2 slideOffset = new Vector2(1000f, 0f);

    Vector2 shownPosition;
    Vector2 hiddenPosition;

    Coroutine animationRoutine;

    void Awake()
    {
        shownPosition = panel.anchoredPosition;
        hiddenPosition = shownPosition + slideOffset;
    }

    public void SlideIn()
    {
        panel.anchoredPosition = hiddenPosition;

        if (animationRoutine != null)
            StopCoroutine(animationRoutine);

        animationRoutine =
            StartCoroutine(Animate(hiddenPosition, shownPosition));
    }

    public void SlideOut()
    {
        if (animationRoutine != null)
            StopCoroutine(animationRoutine);

        animationRoutine =
            StartCoroutine(Animate(shownPosition, hiddenPosition));
    }

    IEnumerator Animate(Vector2 from, Vector2 to)
    {
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;

            float t = timer / duration;

            // Ease Out Back (slight overshoot)
            float c1 = 1.70158f;
            float c3 = c1 + 1f;

            t = 1f + c3 * Mathf.Pow(t - 1f, 3f) + c1 * Mathf.Pow(t - 1f, 2f);

            panel.anchoredPosition =
                Vector2.Lerp(from, to, t);

            yield return null;
        }

        panel.anchoredPosition = to;
    }

    public float Duration => duration;
}