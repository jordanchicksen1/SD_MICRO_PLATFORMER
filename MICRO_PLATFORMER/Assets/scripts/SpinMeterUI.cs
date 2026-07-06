using UnityEngine;
using UnityEngine.UI;

public class SpinMeterUI : MonoBehaviour
{
    [SerializeField] Image fillImage;
    [SerializeField] GameObject meterRoot;
    bool wasVisible;
    bool wasFullyCharged = true;
    Vector3 originalScale;
    Coroutine scaleRoutine;
    Coroutine hideRoutine;

    PlayerCombat combat;

    void Start()
    {
        combat = GetComponentInParent<PlayerCombat>();
        originalScale = meterRoot.transform.localScale;
    }

    void Update()
    {
        fillImage.fillAmount = combat.SpinPercent;

        bool shouldShow =
            combat.IsSpinning || !combat.CanSpin;

        if (shouldShow && !wasVisible)
        {
            PlayPopAnimation();
        }

        wasVisible = shouldShow;

        if (combat.CanSpin && !wasFullyCharged)
        {
            if (hideRoutine != null)
                StopCoroutine(hideRoutine);

            hideRoutine = StartCoroutine(HideAfterDelay());
        }

        wasFullyCharged = combat.CanSpin;

        if (shouldShow)
        {
            if (!meterRoot.activeSelf)
            {
                meterRoot.SetActive(true);
                PlayPopAnimation();
            }

            if (hideRoutine != null)
            {
                StopCoroutine(hideRoutine);
                hideRoutine = null;
            }
        }
    }

    void PlayPopAnimation()
    {
        if (scaleRoutine != null)
            StopCoroutine(scaleRoutine);

        scaleRoutine = StartCoroutine(PopRoutine());
    }

    System.Collections.IEnumerator PopRoutine()
    {
        float timer = 0f;

        float duration = 0.15f;

        Vector3 start = Vector3.zero;
        Vector3 overshoot = originalScale * 1.2f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float t = timer / duration;

            meterRoot.transform.localScale =
                Vector3.Lerp(start, overshoot, t);

            yield return null;
        }

        timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float t = timer / duration;

            meterRoot.transform.localScale =
                Vector3.Lerp(
                    overshoot,
                    originalScale,
                    t);

            yield return null;
        }

        meterRoot.transform.localScale =
            originalScale;
    }

    void PlayPopOutAnimation()
    {
        if (scaleRoutine != null)
            StopCoroutine(scaleRoutine);

        scaleRoutine = StartCoroutine(PopOutRoutine());
    }

    System.Collections.IEnumerator PopOutRoutine()
    {
        float timer = 0f;
        float duration = 0.12f;

        Vector3 overshoot = originalScale * 1.2f;

        // Small pop bigger first
        while (timer < duration)
        {
            timer += Time.deltaTime;

            float t = timer / duration;

            meterRoot.transform.localScale =
                Vector3.Lerp(
                    originalScale,
                    overshoot,
                    t);

            yield return null;
        }

        timer = 0f;

        // Then shrink to nothing
        while (timer < duration)
        {
            timer += Time.deltaTime;

            float t = timer / duration;

            meterRoot.transform.localScale =
                Vector3.Lerp(
                    overshoot,
                    Vector3.zero,
                    t);

            yield return null;
        }

        meterRoot.transform.localScale = Vector3.zero;

        meterRoot.SetActive(false);

        // Reset so the next appearance starts correctly
        meterRoot.transform.localScale = originalScale;
    }

    System.Collections.IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(0.25f);

        if (scaleRoutine != null)
            StopCoroutine(scaleRoutine);

        PlayPopOutAnimation();
    }
}