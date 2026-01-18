using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RotatePlatform90 : MonoBehaviour
{
    [Header("Rotation")]
    [SerializeField] Vector3 rotateAxis = Vector3.up;
    [SerializeField] float rotateAngle = 90f;
    [SerializeField] float rotateDuration = 0.25f;

    [Header("Rider Detection")]
    [SerializeField] float topTolerance = 0.08f; // increase if needed (0.05 - 0.15)

    Rigidbody rb;
    bool isRotating;

    readonly HashSet<Rigidbody> riders = new();

    Vector3 prevPos;
    Quaternion prevRot;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        prevPos = rb.position;
        prevRot = rb.rotation;
    }

    void FixedUpdate()
    {
        Vector3 newPos = rb.position;
        Quaternion newRot = rb.rotation;

        Quaternion deltaRot = newRot * Quaternion.Inverse(prevRot);

        foreach (var rider in riders)
        {
            if (!rider) continue;

            // rotate rider around the platform's previous position
            Vector3 offset = rider.position - prevPos;
            Vector3 rotatedOffset = deltaRot * offset;

            rider.MovePosition(newPos + rotatedOffset);
        }

        prevPos = newPos;
        prevRot = newRot;
    }

    public void RotateNow()
    {
        if (isRotating) return;

        Quaternion from = rb.rotation;
        Quaternion to = from * Quaternion.AngleAxis(rotateAngle, rotateAxis.normalized);
        StartCoroutine(RotateRoutine(from, to));
    }

    IEnumerator RotateRoutine(Quaternion from, Quaternion to)
    {
        isRotating = true;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.fixedDeltaTime / rotateDuration;
            rb.MoveRotation(Quaternion.Slerp(from, to, t));
            yield return new WaitForFixedUpdate();
        }

        rb.MoveRotation(to);
        isRotating = false;
    }

    // ? THIS is the key: collisions from CHILD colliders come here (because RB is on root)
    void OnCollisionStay(Collision collision)
    {
        PlayerController3D player = collision.collider.GetComponent<PlayerController3D>();
        if (!player) return;

        Rigidbody riderRb = player.GetComponent<Rigidbody>();
        if (!riderRb) return;

        Collider playerCol = collision.collider;

        for (int i = 0; i < collision.contactCount; i++)
        {
            ContactPoint contact = collision.contacts[i];

            // This is the PLATFORM CHILD collider that was hit
            Collider platformPieceCol = contact.thisCollider;

            if (!platformPieceCol) continue;

            float playerFeetY = playerCol.bounds.min.y;
            float surfaceTopY = platformPieceCol.bounds.max.y;

            if (playerFeetY >= surfaceTopY - topTolerance)
            {
                riders.Add(riderRb);
                return;
            }
        }
    }

    void OnCollisionExit(Collision collision)
    {
        PlayerController3D player = collision.collider.GetComponent<PlayerController3D>();
        if (!player) return;

        Rigidbody riderRb = player.GetComponent<Rigidbody>();
        if (riderRb)
            riders.Remove(riderRb);
    }
}
