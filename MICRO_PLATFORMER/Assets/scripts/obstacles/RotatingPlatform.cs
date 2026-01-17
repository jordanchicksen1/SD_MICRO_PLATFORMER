using System.Collections;
using UnityEngine;

public class RotatePlatform90 : MonoBehaviour
{
    [SerializeField] Vector3 rotateAxis = Vector3.up;
    [SerializeField] float rotateAngle = 90f;
    [SerializeField] float rotateDuration = 0.25f;

    bool isRotating;

    public void RotateNow()
    {
        if (isRotating) return;

        // ?? Compute a NEW target every time
        Quaternion from = transform.rotation;
        Quaternion to = from * Quaternion.AngleAxis(rotateAngle, rotateAxis.normalized);

        StartCoroutine(RotateRoutine(from, to));
    }

    IEnumerator RotateRoutine(Quaternion from, Quaternion to)
    {
        isRotating = true;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / rotateDuration;
            transform.rotation = Quaternion.Slerp(from, to, t);
            yield return null;
        }

        transform.rotation = to;
        isRotating = false;
    }
}
