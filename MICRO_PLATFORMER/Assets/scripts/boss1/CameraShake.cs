using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance;

    Vector3 startPos;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        startPos = transform.localPosition;
    }

    public static void Shake(float duration, float magnitude)
    {
        if (Instance != null)
        {
            Instance.StartCoroutine(Instance.ShakeRoutine(duration, magnitude));
        }
    }

    IEnumerator ShakeRoutine(float duration, float magnitude)
    {
        float timer = 0f;

        while (timer < duration)
        {
            transform.localPosition =
                startPos + Random.insideUnitSphere * magnitude;

            timer += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = startPos;
    }
}