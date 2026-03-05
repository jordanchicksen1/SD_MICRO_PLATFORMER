using System.Collections;
using UnityEngine;

public class BossStartTrigger : MonoBehaviour
{
    [SerializeField] BossController boss;
    [SerializeField] Transform bossFocusPoint;

    DoorCameraFocus cameraFocus;

    bool triggered;

    void Awake()
    {
        cameraFocus = FindFirstObjectByType<DoorCameraFocus>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;

        triggered = true;

        StartCoroutine(BossIntroSequence());
    }

    IEnumerator BossIntroSequence()
    {
        Debug.Log("Boss Intro Triggered");

        // Move players into arena positions
        yield return boss.StartCoroutine(boss.MovePlayersToArenaStart());

        yield return new WaitForSeconds(0.2f);

        // Focus camera on boss
        if (cameraFocus && bossFocusPoint)
            cameraFocus.FocusOn(bossFocusPoint);

        yield return new WaitForSeconds(0.5f);

        // Boss intro animation
        yield return boss.StartCoroutine(boss.IntroAnimation());

        yield return new WaitForSeconds(0.3f);

        // Start fight
        boss.StartFight();
    }
}