using System.Collections;
using UnityEngine;

public class HubCameraFocus : MonoBehaviour
{
    [Header("Timing")]
    [SerializeField] float moveDuration = 0.55f;
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
    }

    // IMPORTANT: capture defaults AFTER everything is initialized
    void Start()
    {
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
        float fov = (cam && useFovZoom) ? focusedFov : (cam ? cam.fieldOfView : 60f);
        StartMove(focusPoint.position, focusPoint.rotation, fov);
    }

    public void ReturnToDefault()
    {
        StartMove(defaultPos, defaultRot, defaultFov);
    }

    void StartMove(Vector3 targetPos, Quaternion targetRot, float targetFov)
    {
        if (routine != null) StopCoroutine(routine);
        routine = StartCoroutine(MoveRoutine(targetPos, targetRot, targetFov));
    }

    IEnumerator MoveRoutine(Vector3 targetPos, Quaternion targetRot, float targetFov)
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
        if (cam && useFovZoom) cam.fieldOfView = targetFov;

        routine = null;
    }

    public bool IsMoving => routine != null;

    public void ReturnTo(Vector3 pos, Quaternion rot)
    {
        float fov = cam ? cam.fieldOfView : 60f;
        StartMove(pos, rot, fov);
    }


}
