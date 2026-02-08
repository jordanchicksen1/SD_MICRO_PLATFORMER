using System.Collections;
using UnityEngine;

public class HubCameraFocus : MonoBehaviour
{
    [Header("Move")]
    [SerializeField] float moveDuration = 0.6f;
    [SerializeField] AnimationCurve ease = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Zoom (FOV)")]
    [SerializeField] bool useFovZoom = true;
    [SerializeField] float focusedFov = 35f;

    Vector3 defaultPos;
    Quaternion defaultRot;
    float defaultFov;

    Camera cam;
    Coroutine routine;

    void Awake()
    {
        cam = GetComponent<Camera>();
        SaveDefault();
    }

    public void SaveDefault()
    {
        defaultPos = transform.position;
        defaultRot = transform.rotation;
        if (cam) defaultFov = cam.fieldOfView;
    }

    public void FocusOn(Transform focusPoint)
    {
        if (!focusPoint) return;

        if (routine != null) StopCoroutine(routine);
        routine = StartCoroutine(MoveTo(focusPoint.position, focusPoint.rotation, useFovZoom ? focusedFov : (cam ? cam.fieldOfView : 60f)));
    }

    public void ReturnToDefault()
    {
        if (routine != null) StopCoroutine(routine);
        routine = StartCoroutine(MoveTo(defaultPos, defaultRot, defaultFov));
    }

    IEnumerator MoveTo(Vector3 targetPos, Quaternion targetRot, float targetFov)
    {
        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;
        float startFov = cam ? cam.fieldOfView : 60f;

        float t = 0f;
        while (t < moveDuration)
        {
            t += Time.deltaTime;
            float a = ease.Evaluate(Mathf.Clamp01(t / moveDuration));

            transform.position = Vector3.Lerp(startPos, targetPos, a);
            transform.rotation = Quaternion.Slerp(startRot, targetRot, a);

            if (cam && useFovZoom)
                cam.fieldOfView = Mathf.Lerp(startFov, targetFov, a);

            yield return null;
        }

        transform.position = targetPos;
        transform.rotation = targetRot;

        if (cam && useFovZoom)
            cam.fieldOfView = targetFov;

        routine = null;
    }
}
