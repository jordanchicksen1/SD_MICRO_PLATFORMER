using System.Collections;
using UnityEngine;

public class UIPopAnimator : MonoBehaviour
{
    [SerializeField] GameObject uiRoot;

    Vector3 originalScale;
    Coroutine scaleRoutine;

    void Start()
    {
        originalScale = uiRoot.transform.localScale;

        uiRoot.SetActive(false);
        uiRoot.transform.localScale = originalScale;
    }

    public void Show()
    {
        if (scaleRoutine != null)
            StopCoroutine(scaleRoutine);

        uiRoot.SetActive(true);

        scaleRoutine = StartCoroutine(PopRoutine());
    }

    public void Hide()
    {
        if (scaleRoutine != null)
            StopCoroutine(scaleRoutine);

        scaleRoutine = StartCoroutine(PopOutRoutine());
    }

    IEnumerator PopRoutine()
    {
        float timer = 0f;
        float duration = 0.15f;

        Vector3 start = Vector3.zero;
        Vector3 overshoot = originalScale * 1.2f;

        uiRoot.transform.localScale = start;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float t = timer / duration;

            uiRoot.transform.localScale =
                Vector3.Lerp(start, overshoot, t);

            yield return null;
        }

        timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float t = timer / duration;

            uiRoot.transform.localScale =
                Vector3.Lerp(overshoot, originalScale, t);

            yield return null;
        }

        uiRoot.transform.localScale = originalScale;
    }

    IEnumerator PopOutRoutine()
    {
        float timer = 0f;
        float duration = 0.12f;

        Vector3 overshoot = originalScale * 1.2f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float t = timer / duration;

            uiRoot.transform.localScale =
                Vector3.Lerp(originalScale, overshoot, t);

            yield return null;
        }

        timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float t = timer / duration;

            uiRoot.transform.localScale =
                Vector3.Lerp(overshoot, Vector3.zero, t);

            yield return null;
        }

        uiRoot.transform.localScale = Vector3.zero;

        uiRoot.SetActive(false);

        uiRoot.transform.localScale = originalScale;
    }
}