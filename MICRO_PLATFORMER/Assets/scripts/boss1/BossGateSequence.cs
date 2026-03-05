using System.Collections;
using UnityEngine;

public class BossGateSequence : MonoBehaviour
{
    [SerializeField] Door door;

    [Header("Camera Focus")]
    [SerializeField] Transform doorFocusPoint;
    [SerializeField] Transform gemFocusPoint;

    [Header("Gem")]
    [SerializeField] GameObject gemPrefab;
    [SerializeField] Transform gemSpawnPoint;

    DoorCameraFocus cameraFocus;

    bool sequenceRunning;

    void Awake()
    {
        cameraFocus = FindFirstObjectByType<DoorCameraFocus>();
    }

    public void StartSequence()
    {
        if (sequenceRunning) return;

        StartCoroutine(Sequence());
    }

    IEnumerator Sequence()
    {
        sequenceRunning = true;

        // OPEN DOOR
        bool firstTime = door.Open();

        if (firstTime && cameraFocus && doorFocusPoint)
            cameraFocus.FocusOn(doorFocusPoint);

        yield return new WaitForSeconds(2f);

        // SPAWN GEM
        Instantiate(gemPrefab, gemSpawnPoint.position, gemSpawnPoint.rotation);

        // FOCUS GEM
        if (cameraFocus && gemFocusPoint)
            cameraFocus.FocusOn(gemFocusPoint);
    }
}