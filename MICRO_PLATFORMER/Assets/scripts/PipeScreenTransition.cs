using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PipeScreenTransition : MonoBehaviour
{
    [SerializeField] Image transitionImage;

    [Header("Timing")]
    [SerializeField] float closeDuration = 0.5f;
    [SerializeField] float holdDuration = 0.2f;
    [SerializeField] float openDuration = 0.5f;

    bool isTransitioning;

    public bool IsTransitioning => isTransitioning;

    void Awake()
    {
        if (transitionImage != null)
        {
            Color color = transitionImage.color;
            color.a = 0f;
            transitionImage.color = color;
        }
    }

    public IEnumerator Close()
    {
        if (isTransitioning)
            yield break;

        isTransitioning = true;

        yield return StartCoroutine(
            AnimateTransition(
                0f,
                1f,
                closeDuration
            )
        );
    }

    public IEnumerator Open()
    {
        yield return StartCoroutine(
            AnimateTransition(
                1f,
                0f,
                openDuration
            )
        );

        isTransitioning = false;
    }

    IEnumerator AnimateTransition(
        float start,
        float end,
        float duration
    )
    {
        if (transitionImage == null)
            yield break;

        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float t =
                Mathf.Clamp01(timer / duration);

            t = Mathf.SmoothStep(0f, 1f, t);

            Color color =
                transitionImage.color;

            color.a =
                Mathf.Lerp(start, end, t);

            transitionImage.color = color;

            yield return null;
        }

        Color finalColor =
            transitionImage.color;

        finalColor.a = end;

        transitionImage.color = finalColor;
    }
}