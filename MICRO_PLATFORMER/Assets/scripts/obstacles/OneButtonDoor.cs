using UnityEngine;

public class OneButtonDoor : MonoBehaviour
{
    [SerializeField] Door door;
    [SerializeField] ButtonVisual visual;

    [Header("Camera Focus (first open only)")]
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

        if (door == null) return;

        // If you want it to toggle open/close each time:
        // Focus only when it is opening AND it is the first-ever open.
        if (!door.IsOpen)
        {
            bool firstTime = door.Open();
            if (firstTime && cameraFocus && focusPoint)
                cameraFocus.FocusOn(focusPoint);
        }
        else
        {
            door.Close();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        visual?.Release();
    }
}
