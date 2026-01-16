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
        rb.useGravity = true; // gravity can be on, kinematic prevents falling
    }

    void OnCollisionEnter(Collision collision)
    {
        // Only trigger once until respawn
        if (triggered) return;

        // Detect player
        if (!collision.collider.GetComponent<PlayerController3D>())
            return;

        triggered = true;
        StartCoroutine(FallRoutine());
    }

    IEnumerator FallRoutine()
    {
        yield return new WaitForSeconds(fallDelay);

        rb.isKinematic = false; // start falling

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
}
