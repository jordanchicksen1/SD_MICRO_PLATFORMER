using UnityEngine;
using System.Collections;

public class EnemySquash : MonoBehaviour
{
    [SerializeField] float squashY = 0.2f;
    [SerializeField] float stretchX = 1.4f;
    [SerializeField] float squashTime = 0.12f;

    Vector3 startScale;

    void Awake()
    {
        startScale = transform.localScale;
    }

    public void PlaySquash()
    {
        StopAllCoroutines();
        StartCoroutine(SquashRoutine());
    }

    IEnumerator SquashRoutine()
    {
        Vector3 squashed = new Vector3(
            startScale.x * stretchX,
            startScale.y * squashY,
            startScale.z * stretchX
        );

        float t = 0f;
        while (t < squashTime)
        {
            transform.localScale = Vector3.Lerp(startScale, squashed, t / squashTime);
            t += Time.deltaTime;
            yield return null;
        }

        transform.localScale = squashed;
    }
}
