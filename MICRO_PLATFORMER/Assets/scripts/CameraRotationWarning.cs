using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CameraRotationWarning : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Image icon;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip warningSFX;

    [Header("Timing")]
    [SerializeField] float shakeDuration = 0.25f;
    [SerializeField] float displayDuration = 2f;

    [Header("Shake")]
    [SerializeField] float shakeAmount = 10f;

    [Header("Pop Animation")]
    [SerializeField] float popInDuration = 0.15f;
    [SerializeField] float popOutDuration = 0.15f;
    [SerializeField] float popScale = 1.15f;

    RectTransform rectTransform;
    Vector2 originalPosition;
    Vector3 originalScale;

    Coroutine warningRoutine;

    void Awake()
    {
        rectTransform = icon.rectTransform;
        originalPosition = rectTransform.anchoredPosition;
        originalScale = rectTransform.localScale;

        rectTransform.localScale = Vector3.zero;

        SetAlpha(0f);
    }

    public void Show()
    {
        if (warningRoutine != null)
            return;

        warningRoutine = StartCoroutine(WarningRoutine());
    }

    public void Hide()
    {
        if (warningRoutine != null)
        {
            StopCoroutine(warningRoutine);
            warningRoutine = null;
        }

        rectTransform.anchoredPosition =
            originalPosition;

        rectTransform.localScale =
            Vector3.zero;

        SetAlpha(0f);
    }

    IEnumerator WarningRoutine()
    {
        rectTransform.anchoredPosition = originalPosition;

        // Start completely small.
        rectTransform.localScale = Vector3.zero;

        // Make visible.
        SetAlpha(1f);

        // Play sound.
        if (audioSource != null && warningSFX != null)
        {
            audioSource.PlayOneShot(warningSFX);
        }

        // Pop in.
        yield return StartCoroutine(PopInWarning());

        // Shake.
        float timer = 0f;

        while (timer < shakeDuration)
        {
            timer += Time.unscaledDeltaTime;

            Vector2 shake =
                Random.insideUnitCircle * shakeAmount;

            rectTransform.anchoredPosition =
                originalPosition + shake;

            yield return null;
        }

        // Return to normal position.
        rectTransform.anchoredPosition = originalPosition;

        // Stay visible.
        yield return new WaitForSecondsRealtime(displayDuration);

        yield return StartCoroutine(PopOutWarning());

        SetAlpha(0f);

        warningRoutine = null;
    }

    IEnumerator PopInWarning()
    {
        float timer = 0f;

        while (timer < popInDuration)
        {
            timer += Time.unscaledDeltaTime;

            float t =
                Mathf.Clamp01(timer / popInDuration);

            // Smooth the animation.
            t = 1f - Mathf.Pow(1f - t, 3f);

            float scale =
                Mathf.Lerp(
                    0f,
                    popScale,
                    t
                );

            rectTransform.localScale =
                originalScale * scale;

            yield return null;
        }

        // Return to normal size.
        rectTransform.localScale = originalScale;
    }

    IEnumerator PopOutWarning()
    {
        float timer = 0f;

        while (timer < popOutDuration)
        {
            timer += Time.unscaledDeltaTime;

            float t =
                Mathf.Clamp01(timer / popOutDuration);

            // Smooth the animation.
            t = 1f - Mathf.Pow(1f - t, 3f);

            float scale =
                Mathf.Lerp(
                    1f,
                    0f,
                    t
                );

            rectTransform.localScale =
                originalScale * scale;

            yield return null;
        }

        rectTransform.localScale = Vector3.zero;
    }

    void SetAlpha(float alpha)
    {
        if (icon == null)
            return;

        Color color = icon.color;
        color.a = alpha;
        icon.color = color;
    }
}