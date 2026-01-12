using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Transform leftArm;
    [SerializeField] Transform rightArm;
    [SerializeField] Transform leftLeg;
    [SerializeField] Transform rightLeg;
    [SerializeField] Transform body;
    Quaternion leftArmStartRot;
    Quaternion rightArmStartRot;
    Quaternion leftLegStartRot;
    Quaternion rightLegStartRot;

    [Header("Walk Animation")]
    [SerializeField] float armSwingAmount = 30f;
    [SerializeField] float legSwingAmount = 50f; // ? bigger than arms
    [SerializeField] float walkSpeed = 8f;
    [SerializeField] float bodyBobAmount = 0.1f;
    [SerializeField] float bodyBobSpeed = 6f;

    [Header("Jump Pose")]
    [SerializeField] float jumpArmBackAngle = 35f;
    [SerializeField] float jumpArmSideAngle = 10f;
    [SerializeField] float jumpBlendSpeed = 8f;
    [SerializeField] float jumpLean = 15f;
    [SerializeField] float airLegSpread = 20f;
    float jumpBlend; // 0 = grounded, 1 = airborne
   





    Vector3 bodyStartPos;

    Rigidbody rb;

    void Start()
    {
        rb = GetComponentInParent<Rigidbody>();
        bodyStartPos = body.localPosition;
        leftArmStartRot = leftArm.localRotation;
        rightArmStartRot = rightArm.localRotation;
        leftLegStartRot = leftLeg.localRotation;
        rightLegStartRot = rightLeg.localRotation;
    }

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, 1.1f);
    }

    void Update()
    {
        bool grounded = IsGrounded();

        // Smooth blend
        float target = grounded ? 0f : 1f;
        jumpBlend = Mathf.MoveTowards(jumpBlend, target, Time.deltaTime * jumpBlendSpeed);

        AnimateWalk();
        
        AnimateJumpPose(jumpBlend);

        if (rb.linearVelocity.magnitude < 0.1f && grounded)
        {
            float breathe = Mathf.Sin(Time.time * 2f) * 0.05f;
            body.localPosition = bodyStartPos + Vector3.up * breathe;
        }
    }

    void AnimateWalk()
    {
        float speed = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z).magnitude;

        if (speed < 0.1f)
            return;

        float t = Time.time * walkSpeed;

        float armSwing = Mathf.Sin(t) * armSwingAmount;
        float legSwing = Mathf.Sin(t) * legSwingAmount;

        // Arms (slightly softer)
        leftArm.localRotation = leftArmStartRot * Quaternion.Euler(armSwing, 0, 0);
        rightArm.localRotation = rightArmStartRot * Quaternion.Euler(-armSwing, 0, 0);

        // Legs (walk ? air blend)
        Quaternion walkLeftLeg =
            leftLegStartRot * Quaternion.Euler(-legSwing, 0, 0);

        Quaternion walkRightLeg =
            rightLegStartRot * Quaternion.Euler(legSwing, 0, 0);

        Quaternion airLeftLeg =
            leftLegStartRot * Quaternion.Euler(airLegSpread, 0, 0);

        Quaternion airRightLeg =
            rightLegStartRot * Quaternion.Euler(airLegSpread, 0, 0);

        leftLeg.localRotation =
            Quaternion.Lerp(walkLeftLeg, airLeftLeg, jumpBlend);

        rightLeg.localRotation =
            Quaternion.Lerp(walkRightLeg, airRightLeg, jumpBlend);


        body.localPosition =
            bodyStartPos + Vector3.up * Mathf.Sin(t * bodyBobSpeed) * bodyBobAmount;
    }


    void AnimateJumpPose(float blend)
    {
        // Body lean
        body.localRotation = Quaternion.Lerp(
            Quaternion.identity,
            Quaternion.Euler(jumpLean, 0, 0),
            blend
        );

        
        // Arms swing back
        Quaternion leftJumpArm =
            leftArmStartRot * Quaternion.Euler(-jumpArmBackAngle, -jumpArmSideAngle, 0);

        Quaternion rightJumpArm =
            rightArmStartRot * Quaternion.Euler(-jumpArmBackAngle, jumpArmSideAngle, 0);

        leftArm.localRotation = Quaternion.Lerp(
            leftArm.localRotation,
            leftJumpArm,
            blend
        );

        rightArm.localRotation = Quaternion.Lerp(
            rightArm.localRotation,
            rightJumpArm,
            blend
        );
    }

    

}
