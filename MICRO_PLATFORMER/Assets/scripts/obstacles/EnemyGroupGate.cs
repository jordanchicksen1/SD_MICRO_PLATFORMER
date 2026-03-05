using UnityEngine;

public class EnemyGroupGate : MonoBehaviour
{
    [SerializeField] Enemy[] enemies;
    [SerializeField] Door door;

    [Header("Camera Focus")]
    [SerializeField] Transform focusPoint;

    DoorCameraFocus cameraFocus;

    bool opened;

    void Awake()
    {
        cameraFocus = FindFirstObjectByType<DoorCameraFocus>();
    }

    void Update()
    {
        if (opened) return;

        foreach (Enemy e in enemies)
        {
            if (e != null && !e.IsDead)
                return;
        }

        opened = true;

        bool firstTime = door.Open();

        if (firstTime && cameraFocus && focusPoint)
            cameraFocus.FocusOn(focusPoint);
    }
}