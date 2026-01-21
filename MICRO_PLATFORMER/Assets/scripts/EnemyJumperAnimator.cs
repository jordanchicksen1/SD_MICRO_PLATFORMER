using UnityEngine;

public class EnemyJumperAnimator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Transform body;

    [Header("Legs")]
    [SerializeField] Transform frontLeftLeg;
    [SerializeField] Transform frontRightLeg;
    [SerializeField] Transform backLeftLeg;
    [SerializeField] Transform backRightLeg;

    [Header("Walk")]
    [SerializeField] float legSwingAmount = 35f;
    [SerializeField] float walkSpeed = 7f;
    [SerializeField] float bodyBob = 0.05f;

    [Header("Idle")]
    [SerializeField] float idleBodyBob = 0.03f;
    [SerializeField] float idleSpeed = 2f;

    Rigidbody rb;
    Vector3 bodyStartPos;

    Quaternion flStart, frStart, blStart, brStart;

    void Start()
    {
        rb = GetComponentInParent<Rigidbody>();

        bodyStartPos = body.localPosition;

        flStart = frontLeftLeg.localRotation;
        frStart = frontRightLeg.localRotation;
        blStart = backLeftLeg.localRotation;
        brStart = backRightLeg.localRotation;
    }

    void Update()
    {
        float speed = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z).magnitude;

        if (speed < 0.1f)
        {
            AnimateIdle();
        }
        else
        {
            AnimateWalk();
        }
    }

    void AnimateIdle()
    {
        // legs straight (start pose)
        frontLeftLeg.localRotation = Quaternion.Lerp(frontLeftLeg.localRotation, flStart, Time.deltaTime * 10f);
        frontRightLeg.localRotation = Quaternion.Lerp(frontRightLeg.localRotation, frStart, Time.deltaTime * 10f);
        backLeftLeg.localRotation = Quaternion.Lerp(backLeftLeg.localRotation, blStart, Time.deltaTime * 10f);
        backRightLeg.localRotation = Quaternion.Lerp(backRightLeg.localRotation, brStart, Time.deltaTime * 10f);

        float breathe = Mathf.Sin(Time.time * idleSpeed) * idleBodyBob;
        body.localPosition = bodyStartPos + Vector3.up * breathe;
    }

    void AnimateWalk()
    {
        float t = Time.time * walkSpeed;

        // Basic quadruped gait:
        // FL + BR together, FR + BL together
        float a = Mathf.Sin(t) * legSwingAmount;
        float b = Mathf.Sin(t + Mathf.PI) * legSwingAmount;

        frontLeftLeg.localRotation = flStart * Quaternion.Euler(-a, 0, 0);
        backRightLeg.localRotation = brStart * Quaternion.Euler(a, 0, 0);

        frontRightLeg.localRotation = frStart * Quaternion.Euler(-b, 0, 0);
        backLeftLeg.localRotation = blStart * Quaternion.Euler(b, 0, 0);

        body.localPosition = bodyStartPos + Vector3.up * Mathf.Abs(Mathf.Sin(t)) * bodyBob;
    }
}
