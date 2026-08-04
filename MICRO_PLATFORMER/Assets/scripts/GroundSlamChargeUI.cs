using UnityEngine;
using UnityEngine.UI;

public class GroundSlamChargeUI : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    [SerializeField] private GameObject meterRoot;

    private bool wasVisible;
    private bool wasFullyCharged = false;

    private Vector3 originalScale;

    private Coroutine scaleRoutine;

    private PlayerCombat combat;

    void Start()
    {
        combat = GetComponentInParent<PlayerCombat>();

        originalScale = meterRoot.transform.localScale;

        meterRoot.SetActive(false);
    }

    void Update()
    {
        fillImage.fillAmount = combat.GroundSlamChargePercent;

        bool shouldShow =
     combat.ShowGroundSlamUI &&
     !combat.CanGroundSlam;

        // Pop in when charging starts
        if (shouldShow && !wasVisible)
        {
            meterRoot.SetActive(true);
            PlayPopAnimation();
        }

        // Pop out immediately when fully charged
        if (combat.CanGroundSlam && !wasFullyCharged)
        {
            PlayPopOutAnimation();
        }

        wasVisible = shouldShow;
        wasFullyCharged = combat.CanGroundSlam;
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

        meterRoot.transform.localScale = originalScale;
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

        meterRoot.transform.localScale = originalScale;
    }
}