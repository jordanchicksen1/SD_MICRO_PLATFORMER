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

    [Header("Boss Button")]
    [SerializeField] GameObject bossButton;
    [SerializeField] Transform buttonFocusPoint;
    [SerializeField] Transform buttonVFXPoint;
    [SerializeField] GameObject buttonVFX;

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

        // -------------------------
        // OPEN DOOR
        // -------------------------

        bool firstTime = door.Open();

        if (firstTime && cameraFocus && doorFocusPoint)
            cameraFocus.FocusOn(doorFocusPoint);

        yield return new WaitForSeconds(2f);


        // -------------------------
        // SPAWN GEM
        // -------------------------

        Instantiate(
            gemPrefab,
            gemSpawnPoint.position,
            gemSpawnPoint.rotation
        );

        // FOCUS GEM
        if (cameraFocus && gemFocusPoint)
            cameraFocus.FocusOn(gemFocusPoint);

        yield return new WaitForSeconds(2f);


        // -------------------------
        // ACTIVATE BUTTON
        // -------------------------

        if (bossButton != null)
            bossButton.SetActive(true);


        // -------------------------
        // BUTTON SMOKE
        // -------------------------

        if (buttonVFX != null)
        {
            Transform spawnPoint =
                buttonVFXPoint != null
                    ? buttonVFXPoint
                    : bossButton.transform;

            Instantiate(
                buttonVFX,
                spawnPoint.position,
                spawnPoint.rotation
            );
        }


        // -------------------------
        // FOCUS BUTTON
        // -------------------------

        if (cameraFocus && buttonFocusPoint)
            cameraFocus.FocusOn(buttonFocusPoint);
    }
}