using UnityEngine;

public class EnemyQuadrupedAnimator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Transform body;
    [SerializeField] Transform frontLeft;
    [SerializeField] Transform frontRight;
    [SerializeField] Transform backLeft;
    [SerializeField] Transform backRight;

    [Header("Animation")]
    [SerializeField] float legSwing = 30f;
    [SerializeField] float walkSpeed = 6f;
    [SerializeField] float bodyBobAmount = 0.08f;
    [SerializeField] float idleLegResetSpeed = 8f;

    Rigidbody rb;
    Vector3 bodyStartPos;

    Quaternion flStart, frStart, blStart, brStart;

    void Start()
    {
        rb = GetComponentInParent<Rigidbody>();

        bodyStartPos = body.localPosition;

        flStart = frontLeft.localRotation;
        frStart = frontRight.localRotation;
        blStart = backLeft.localRotation;
        brStart = backRight.localRotation;
    }

    void Update()
    {
        float speed = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z).magnitude;

        if (speed < 0.1f)
        {
            Idle();
            return;
        }

        Walk();
    }

    void Idle()
    {
        // Subtle body breathing
        float breathe = Mathf.Sin(Time.time * 2f) * 0.03f;
        body.localPosition = bodyStartPos + Vector3.up * breathe;

        // Legs perfectly straight
        frontLeft.localRotation =
            Quaternion.Slerp(frontLeft.localRotation, flStart, Time.deltaTime * idleLegResetSpeed);

        frontRight.localRotation =
            Quaternion.Slerp(frontRight.localRotation, frStart, Time.deltaTime * idleLegResetSpeed);

        backLeft.localRotation =
            Quaternion.Slerp(backLeft.localRotation, blStart, Time.deltaTime * idleLegResetSpeed);

        backRight.localRotation =
            Quaternion.Slerp(backRight.localRotation, brStart, Time.deltaTime * idleLegResetSpeed);
    }


    void Walk()
    {
        float t = Time.time * walkSpeed;
        float swing = Mathf.Sin(t) * legSwing;

        // Diagonal pairing (Minecraft pig style)
        frontLeft.localRotation = flStart * Quaternion.Euler(swing, 0, 0);
        backRight.localRotation = brStart * Quaternion.Euler(swing, 0, 0);

        frontRight.localRotation = frStart * Quaternion.Euler(-swing, 0, 0);
        backLeft.localRotation = blStart * Quaternion.Euler(-swing, 0, 0);

        body.localPosition =
            bodyStartPos + Vector3.up * Mathf.Abs(Mathf.Sin(t)) * bodyBobAmount;
    }
}
