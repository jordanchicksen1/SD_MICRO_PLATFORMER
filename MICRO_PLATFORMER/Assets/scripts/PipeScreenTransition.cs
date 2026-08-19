using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PipeScreenTransition : MonoBehaviour
{
    [SerializeField] Image irisImage;

    [Header("Timing")]
    [SerializeField] float transitionDuration = 0.6f;

    Material irisMaterial;

    static readonly int IrisProperty =
        Shader.PropertyToID("_Iris");

    void Awake()
    {
        if (irisImage == null)
            return;

        irisMaterial =
            new Material(irisImage.material);

        irisImage.material =
            irisMaterial;

        SetIris(0f);
    }

    public IEnumerator Close()
    {
        yield return StartCoroutine(
            AnimateIris(0f, 1f)
        );

        // Make absolutely sure the screen
        // is completely closed.
        SetIris(1f);
    }

    public IEnumerator Open()
    {
        // Start completely closed.
        SetIris(1f);

        yield return StartCoroutine(
            AnimateIris(1f, 0f)
        );

        // Make absolutely sure the screen
        // is completely open.
        SetIris(0f);
    }

    IEnumerator AnimateIris(
        float start,
        float end
    )
    {
        float timer = 0f;

        while (timer < transitionDuration)
        {
            timer += Time.deltaTime;

            float t =
                Mathf.Clamp01(
                    timer / transitionDuration
                );

            // Same curve in both directions.
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