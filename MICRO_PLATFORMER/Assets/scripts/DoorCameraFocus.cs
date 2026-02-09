using System.Collections;
using UnityEngine;

public class DoorCameraFocus : MonoBehaviour
{
    [SerializeField] CoopCameraController coopCam;

    [Header("Timing")]
    [SerializeField] float moveDuration = 0.6f;
    [SerializeField] float holdTime = 1.0f;
    [SerializeField] AnimationCurve ease = AnimationCurve.EaseInOut(0, 0, 1, 1);

    Coroutine routine;

    void Awake()
    {
        if (!coopCam) coopCam = GetComponent<CoopCameraController>();
    }

    public void FocusOn(Transform focusPoint)
    {
        if (!focusPoint) return;

        if (routine != null) StopCoroutine(routine);
        routine = StartCoroutine(FocusRoutine(focusPoint));
    }

    IEnumerator FocusRoutine(Transform focusPoint)
    {
        if (coopCam) coopCam.cutsceneActive = true;

        Transform pivot = transform.GetChild(0);
        Camera cam = pivot.GetComponentInChildren<Camera>();

        // Save current camera rig state
        Vector3 startPos = transform.position;
        Quaternion startPivotRot = pivot.rotation;
        Vector3 startCamLocalPos = cam.transform.localPosition;

        // Target state comes from the focusPoint
        Vector3 targetPos = focusPoint.position;
        Quaternion targetPivotRot = focusPoint.rotation;

        // Optional: let the focusPoint decide zoom using its child "CamOffset"
        // If you don’t want this yet, just set targetCamLocalPos = startCamLocalPos;
        Vector3 targetCamLocalPos = startCamLocalPos;

        float t = 0f;
        while (t < moveDuration)
        {
            t += Time.deltaTime;
            float a = ease.Evaluate(Mathf.Clamp01(t / moveDuration));

            transform.position = Vector3.Lerp(startPos, targetPos, a);
            pivot.rotation = Quaternion.Slerp(startPivotRot, targetPivotRot, a);
            cam.transform.localPosition = Vector3.Lerp(startCamLocalPos, targetCamLocalPos, a);

            yield return null;
        }

        transform.position = targetPos;
        pivot.rotation = targetPivotRot;
        cam.transform.localPosition = targetCamLocalPos;

        // Hold on the door
        yield return new WaitForSeconds(holdTime);

        // Return
        t = 0f;
        while (t < moveDuration)
        {
            t += Time.deltaTime;
            float a = ease.Evaluate(Mathf.Clamp01(t / moveDuration));

            transform.position = Vector3.Lerp(targetPos, startPos, a);
            pivot.rotation = Quaternion.Slerp(targetPivotRot, startPivotRot, a);
            cam.transform.localPosition = Vector3.Lerp(targetCamLocalPos, startCamLocalPos, a);

            yield return null;
        }

        transform.position = startPos;
        pivot.rotation = startPivotRot;
        cam.transform.localPosition = startCamLocalPos;

        if (coopCam) coopCam.cutsceneActive = false;
        routine = null;
    }
}
