using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FallingPlatform : MonoBehaviour
{
    [Header("Timings")]
    [SerializeField] float fallDelay = 0.5f;
    [SerializeField] float respawnDelay = 2.5f;

    [Header("Fall")]
    [SerializeField] bool disableColliderWhileFallen = true;

    [Header("Shake Warning")]
    [SerializeField] bool shakeBeforeFall = true;
    [SerializeField] float shakeDuration = 0.45f;     // usually a bit less than fallDelay
    [SerializeField] float shakeIntensity = 0.06f;    // how far it jitters (units)
    [SerializeField] float shakeFrequency = 35f;      // how fast it jitters
    [SerializeField] AnimationCurve shakeRamp = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Optional SFX")]
    [SerializeField] AudioSource shakeSFX;

    Rigidbody rb;
    Collider col;

    Vector3 startPos;
    Quaternion startRot;

    bool triggered;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();

        startPos = transform.position;
        startRot = transform.rotation;

        rb.isKinematic = true;
        rb.useGravity = true;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (triggered) return;

        // Detect player (safe if collider is a child)
        if (!collision.collider.GetComponentInParent<PlayerController3D>())
            return;

        triggered = true;
        StartCoroutine(FallRoutine());
    }

    IEnumerator FallRoutine()
    {
        // Warning shake phase (during the delay)
        if (shakeBeforeFall)
        {
            if (shakeSFX && shakeSFX.clip) shakeSFX.Play();
            yield return StartCoroutine(ShakeRoutine(Mathf.Min(shakeDuration, fallDelay)));
            if (shakeSFX && shakeSFX.isPlaying) shakeSFX.Stop();
        }
        else
        {
            yield return new WaitForSeconds(fallDelay);
        }

        // Fall
        if (disableColliderWhileFallen && col != null)
            //col.enabled = false;

        rb.isKinematic = false;

        yield return new WaitForSeconds(respawnDelay);

        // Reset
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        rb.isKinematic = true;
        transform.SetPositionAndRotation(startPos, startRot);

        if (disableColliderWhileFallen && col != null)
            col.enabled = true;

        triggered = false;
    }

    IEnumerator ShakeRoutine(float duration)
    {
        Vector3 basePos = startPos; // keep it deterministic

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float a = duration <= 0f ? 1f : Mathf.Clamp01(t / duration);

            // ramp up shake so it feels like "about to drop"
            float ramp = shakeRamp != null ? shakeRamp.Evaluate(a) : a;

            // jitter on XZ only (usually looks best for platforms)
            float x = (Mathf.PerlinNoise(Time.time * shakeFrequency, 0f) - 0.5f) * 2f;
            float z = (Mathf.PerlinNoise(0f, Time.time * shakeFrequency) - 0.5f) * 2f;

            Vector3 offset = new Vector3(x, 0f, z) * (shakeIntensity * ramp);
            transform.position = basePos + offset;

            yield return null;
        }

        // snap back cleanly
        transform.position = basePos;
    }

    // Optional: if you move platforms in-editor after play starts, keep startPos accurate
    void OnValidate()
    {
        if (shakeDuration > fallDelay) shakeDuration = fallDelay;
    }
}
