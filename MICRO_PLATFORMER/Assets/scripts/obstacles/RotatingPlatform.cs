using System.Collections;
using UnityEngine;

public class RotatePlatform90 : MonoBehaviour
{
    [SerializeField] Vector3 rotateAxis = Vector3.up; // rotate around Y by default
    [SerializeField] float rotateAngle = 90f;
    [SerializeField] float rotateDuration = 0.25f;

    bool isRotating;
    Quaternion startRot;
    Quaternion targetRot;

    void Awake()
    {
        startRot = transform.rotation;
        targetRot = startRot * Quaternion.AngleAxis(rotateAngle, rotateAxis.normalized);
    }

    public void RotateNow()
    {
        if (isRotating) return;
        StartCoroutine(RotateRoutine());
    }

    IEnumerator RotateRoutine()
    {
        isRotating = true;

        Quaternion from = transform.rotation;
        Quaternion to = targetRot;

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
