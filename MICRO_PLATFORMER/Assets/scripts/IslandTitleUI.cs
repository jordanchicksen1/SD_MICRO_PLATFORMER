using System.Collections;
using TMPro;
using UnityEngine;

public class IslandTitleUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] TMP_Text titleText;

    [Header("Timing")]
    [SerializeField] float fadeInDuration = 0.5f;
    [SerializeField] float displayDuration = 5f;
    [SerializeField] float fadeOutDuration = 0.5f;

    Coroutine titleRoutine;

    void Awake()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        canvasGroup.alpha = 0f;
    }

    public void ShowTitle(string title)
    {
        if (titleRoutine != null)
            StopCoroutine(titleRoutine);

        titleRoutine =
            StartCoroutine(ShowTitleRoutine(title));
    }

    IEnumerator ShowTitleRoutine(string title)
    {
        titleText.text = title;

        canvasGroup.alpha = 0f;

        // Fade in.
        float timer = 0f;

        while (timer < fadeInDuration)
        {
            timer += Time.deltaTime;

            canvasGroup.alpha =
                Mathf.Clamp01(
                    timer / fadeInDuration
                );

            yield return null;
        }

        canvasGroup.alpha = 1f;

        // Stay visible.
        yield return new WaitForSeconds(
            displayDuration
        );

        // Fade out.
        timer = 0f;

        while (timer < fadeOutDuration)
        {
            timer += Time.deltaTime;

            canvasGroup.alpha =
                1f - Mathf.Clamp01(
                    timer / fadeOutDuration
                );

            yield return null;
        }

        canvasGroup.alpha = 0f;

        titleRoutine = null;
    }
}