using UnityEngine;
using System.Collections;

public class RotatorButton : MonoBehaviour
{
    [SerializeField] RotatePlatform90 targetPlatform;
    [SerializeField] ButtonVisual visual;
    [SerializeField] float resetTime = 0.5f;

    bool pressed;

    void Awake()
    {
        if (visual == null) visual = GetComponent<ButtonVisual>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (pressed) return;

        PlayerController3D player = other.GetComponent<PlayerController3D>();
        if (!player) return;

        // Optional: only if falling / landing
        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb != null && rb.linearVelocity.y > 0.1f)
            return;

        pressed = true;
        visual?.Press();

        targetPlatform?.RotateNow();

        StartCoroutine(ResetButton());
    }

    IEnumerator ResetButton()
    {
        yield return new WaitForSeconds(resetTime);
        pressed = false;
        visual?.Release();
    }
}
