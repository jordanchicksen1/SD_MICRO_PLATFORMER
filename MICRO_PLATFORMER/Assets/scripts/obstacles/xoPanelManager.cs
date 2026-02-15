using UnityEngine;

public class QuestionPanelManager : MonoBehaviour
{
    [SerializeField] int requiredCount = 3;
    [SerializeField] DoorOpenSimple door;
    public AudioSource buttonSFX;
    public AudioSource doorSFX;

    [Header("Badges")]
    public GameObject xBadge;
    public GameObject oBadge;

    [Header("Camera Focus (first open only)")]
    [SerializeField] DoorCameraFocus cameraFocus;
    [SerializeField] Transform focusPoint;

    int currentCount;
    bool doorOpened;

    void Awake()
    {
        if (!cameraFocus)
            cameraFocus = FindFirstObjectByType<DoorCameraFocus>();
    }

    public void AddPanel()
    {
        currentCount++;
        Debug.Log($"Panels: {currentCount}/{requiredCount}");
        CheckDoor();
        buttonSFX.Play();
    }

    public void RemovePanel()
    {
        currentCount--;
        if (currentCount < 0) currentCount = 0;
        buttonSFX.Play();
        Debug.Log($"Panels: {currentCount}/{requiredCount}");
    }

    void CheckDoor()
    {
        if (doorOpened) return;

        if (currentCount >= requiredCount)
        {
            doorOpened = true;

            // Open door
            if (door != null) door.Open();
            doorSFX.Play();

            // Camera focus shot (once)
            if (cameraFocus && focusPoint)
                cameraFocus.FocusOn(focusPoint);

            // Badge swap
            if (xBadge) xBadge.SetActive(false);
            if (oBadge) oBadge.SetActive(true);
        }
    }
}
