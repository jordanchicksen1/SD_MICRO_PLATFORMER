using UnityEngine;

public class OneButtonDoor : MonoBehaviour
{
    [SerializeField] Door door;
    [SerializeField] ButtonVisual visual;

    [Header("Camera Focus")]
    [SerializeField] DoorCameraFocus cameraFocus;
    [SerializeField] Transform focusPoint;

    void Awake()
    {
        if (visual == null) visual = GetComponent<ButtonVisual>();
        if (!cameraFocus) cameraFocus = FindFirstObjectByType<DoorCameraFocus>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        visual?.Press();
        door?.Toggle();

        if (cameraFocus && focusPoint)
            cameraFocus.FocusOn(focusPoint);
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        visual?.Release();
    }
}
