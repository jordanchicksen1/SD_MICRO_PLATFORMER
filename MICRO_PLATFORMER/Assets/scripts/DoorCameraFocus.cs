using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DoorCameraFocus : MonoBehaviour
{
    [SerializeField] CoopCameraController coopCam;

    [Header("Timing")]
    [SerializeField] float moveDuration = 0.6f;
    [SerializeField] float holdTime = 1.0f;
    [SerializeField] AnimationCurve ease = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Optional polish: freeze players during focus")]
    [SerializeField] bool freezePlayers = true;

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

        // Gather player inputs + rigidbodies so we can freeze properly
        List<PlayerInput> inputs = new();
        List<Rigidbody> rbs = new();

        if (freezePlayers && coopCam != null)
        {
            foreach (var p in coopCam.players)
            {
                if (!p) continue;

                var pi = p.GetComponentInParent<PlayerInput>();
                if (pi && !inputs.Contains(pi)) inputs.Add(pi);

                var rb = p.GetComponentInParent<Rigidbody>();
                if (rb && !rbs.Contains(rb)) rbs.Add(rb);
            }

            // Stop motion immediately (prevents drifting while inputs are off)
            foreach (var rb in rbs)
                rb.linearVelocity = Vector3.zero;

            // Disable input
            foreach (var pi in inputs)
                if (pi.enabled) pi.DeactivateInput();
        }

        Transform pivot = transform.GetChild(0);
        Camera cam = pivot.GetComponentInChildren<Camera>();

        // Save current camera rig state
        Vector3 startPos = transform.position;
        Quaternion startPivotRot = pivot.rotation;
        Vector3 startCamLocalPos = cam.transform.localPosition;

        // Target state comes from focusPoint
        Vector3 targetPos = focusPoint.position;
        Quaternion targetPivotRot = focusPoint.rotation;

        // (Optional) keep zoom the same during focus
        Vector3 targetCamLocalPos = startCamLocalPos;

        // Move in
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

        // Hold (use realtime so it still works if timeScale changes later)
        yield return new WaitForSecondsRealtime(holdTime);

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

        // Re-enable input
        if (freezePlayers)
        {
            foreach (var pi in inputs)
                if (pi && pi.enabled) pi.ActivateInput();
        }

        routine = null;
    }
}
