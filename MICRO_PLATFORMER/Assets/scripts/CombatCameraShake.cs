using UnityEngine;
using System.Collections;

public class CombatCameraShake : MonoBehaviour
{
    Coroutine shakeRoutine;

    public void Shake(float duration, float magnitude)
    {
        if (shakeRoutine != null)
            StopCoroutine(shakeRoutine);

        shakeRoutine = StartCoroutine(
            ShakeRoutine(duration, magnitude));
    }

    IEnumerator ShakeRoutine(float duration, float magnitude)
    {
        Vector3 originalPosition = transform.localPosition;

        float timer = 0f;

        while (timer < duration)
        {
            Vector2 offset = Random.insideUnitCircle * magnitude;

            transform.localPosition =
                originalPosition +
                new Vector3(offset.x, offset.y, 0f);

            timer += Time.deltaTime;

            yield return null;
        }

        transform.localPosition = originalPosition;
    }
}