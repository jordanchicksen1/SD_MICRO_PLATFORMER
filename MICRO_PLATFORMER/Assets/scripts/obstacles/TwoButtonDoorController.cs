using UnityEngine;

public class CoopDoorController : MonoBehaviour
{
    [SerializeField] CoopDoor door;
    [SerializeField] ToggleButton buttonA;
    [SerializeField] ToggleButton buttonB;

    [Header("Camera Focus (first open only)")]
    [SerializeField] DoorCameraFocus cameraFocus;
    [SerializeField] Transform focusPoint; // create & assign an empty transform near the door

    void Awake()
    {
        if (door == null) door = GetComponent<CoopDoor>();
        if (!cameraFocus) cameraFocus = FindFirstObjectByType<DoorCameraFocus>();
    }

    public void RecomputeDoor()
    {
        bool open = buttonA != null && buttonB != null &&
                    buttonA.IsPressed && buttonB.IsPressed;

        if (door == null) return;

        bool firstTimeOpened = door.SetOpen(open);

        // Only play focus shot the very first time this door opens
        if (firstTimeOpened && cameraFocus && focusPoint)
            cameraFocus.FocusOn(focusPoint);
    }
}
