using UnityEngine;
using System.Collections;

public class DoorOpen : MonoBehaviour
{
    [SerializeField] Vector3 openOffset = new Vector3(0, 3f, 0);
    [SerializeField] float openTime = 0.4f;

    [Header("Audio")]
    [SerializeField] AudioSource doorSFX;

    [Header("Camera Focus (Optional)")]
    [SerializeField] DoorCameraFocus cameraFocus;
    [SerializeField] Transform focusPoint;
    [SerializeField] bool focusOnlyFirstTime = true;

    Vector3 closedPos;
    Vector3 openPos;

    bool isOpen;
    bool openedEver;

    void Awake()
    {
        closedPos = transform.position;
        openPos = closedPos + openOffset;

        if (!cameraFocus)
            cameraFocus = FindFirstObjectByType<DoorCameraFocus>();
    }

    public void Open()
    {
        if (isOpen) return;

        isOpen = true;

        bool firstTime = !openedEver;
        openedEver = true;

        StopAllCoroutines();
        StartCoroutine(MoveDoor(closedPos, openPos));

        // Camera focus logic
        if (cameraFocus && focusPoint)
        {
            if (!focusOnlyFirstTime || firstTime)
                cameraFocus.FocusOn(focusPoint);
        }
    }

    IEnumerator MoveDoor(Vector3 from, Vector3 to)
    {
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / openTime;
            transform.position = Vector3.Lerp(from, to, t);
            yield return null;
        }

        transform.position = to;

        if (doorSFX)
            doorSFX.Play();
    }
}
