using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PipeScreenTransition : MonoBehaviour
{
    [SerializeField] Image irisImage;

    [Header("Timing")]
    [SerializeField] float closeDuration = 0.5f;
    [SerializeField] float openDuration = 0.6f;

    Material irisMaterial;

    static readonly int IrisProperty =
        Shader.PropertyToID("_Iris");

    void Awake()
    {
        if (irisImage == null)
            return;

        // Create an instance so we don't modify
        // the original material asset.
        irisMaterial =
            new Material(irisImage.material);

        irisImage.material =
            irisMaterial;

        SetIris(0f);
    }

    public IEnumerator Close()
    {
        yield return StartCoroutine(
            AnimateIris(
                0f,
                1f,
                closeDuration
            )
        );
    }

    public IEnumerator Open()
    {
        yield return StartCoroutine(
            AnimateIris(
                1f,
                0f,
                openDuration
            )
        );
    }

    IEnumerator AnimateIris(
        float start,
        float end,
        float duration
    )
    {
        if (irisMaterial == null)
            yield break;

        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float t =
                Mathf.Clamp01(
                    timer / duration
                );

            t = Mathf.SmoothStep(
                0f,
                1f,
                t
            );

            float value =
                Mathf.Lerp(
                    start,
                    end,
                    t
                );

            SetIris(value);

            yield return null;
        }

        SetIris(end);
    }

    void SetIris(float value)
    {
        if (irisMaterial == null)
            return;

        irisMaterial.SetFloat(
            IrisProperty,
            value
        );
    }
}