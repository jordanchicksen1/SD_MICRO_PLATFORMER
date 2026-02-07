using UnityEngine;

public class HubPlayerAnimator : MonoBehaviour
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

    [Header("Idle Animation")]
    [SerializeField] float idleArmBobAmount = 6f;
    [SerializeField] float idleArmBobSpeed = 2f;
    [SerializeField] float idleBlendSpeed = 6f;

    float idleBlend; // 0 = active, 1 = idle


    [Header("Walk Animation")]
    [SerializeField] float armSwingAmount = 30f;
    [SerializeField] float legSwingAmount = 50f; // ? bigger than arms
    [SerializeField] float walkSpeed = 8f;
    [SerializeField] float bodyBobAmount = 0.1f;
    [SerializeField] float bodyBobSpeed = 6f;

    float externalMoveBlend; // 0..1
    public void SetMoveBlend(float v) => externalMoveBlend = Mathf.Clamp01(v);


    [Header("Jump Pose")]
    [SerializeField] float jumpArmBackAngle = 35f;
    [SerializeField] float jumpArmSideAngle = 10f;
    [SerializeField] float jumpBlendSpeed = 8f;
    [SerializeField] float jumpLean = 15f;
    [SerializeField] float airLegSpread = 20f;
    float jumpBlend; // 0 = grounded, 1 = airborne

    [Header("Dive Pose")]
    [SerializeField] float diveArmForwardAngle = 65f;
    [SerializeField] float diveBodyPitch = 40f;
    [SerializeField] float diveLegBackAngle = 20f;
    [SerializeField] float diveBlendSpeed = 10f;

    float diveBlend;
    bool isDiving;

    [Header("Ground Pound Pose")]
    [SerializeField] float poundArmDownAngle = -80f;
    [SerializeField] float poundLegStraightAngle = 0f;
    [SerializeField] float poundBodyLean = 25f;
    [SerializeField] float poundBlendSpeed = 12f;

    float poundBlend;
    bool isGroundPounding;

    [Header("Carry Pose")]
    [SerializeField] float carryArmForwardAngle = 55f;   // tweak in inspector
    [SerializeField] float carryArmOutAngle = 10f;       // slight outward spread
    [SerializeField] float carryBlendSpeed = 12f;

    bool isCarrying;
    float carryBlend;

    [Header("Gem Pose")]
    [SerializeField] float gemArmUpAngle = -120f;
    [SerializeField] float gemBodyLean = -10f;
    [SerializeField] float gemBlendSpeed = 8f;

    float gemBlend;
    bool isGemPose;

    [SerializeField] Collider groundCollider;
    [SerializeField] LayerMask groundMask;
    [SerializeField] float groundCheckExtra = 0.08f;
    [SerializeField] float groundCheckRadiusScale = 0.9f;


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
        if (rb == null) return false;

        if (groundMask == 0)
            groundMask = LayerMask.GetMask("GroundLayer");

        if (groundCollider == null)
            groundCollider = GetComponentInParent<Collider>();

        if (groundCollider == null) return false;

        Bounds b = groundCollider.bounds;

        float radius = Mathf.Max(0.05f, Mathf.Min(b.extents.x, b.extents.z) * groundCheckRadiusScale);
        Vector3 origin = new Vector3(b.center.x, b.min.y + radius + 0.02f, b.center.z);

        return Physics.SphereCast(
            origin,
            radius,
            Vector3.down,
            out _,
            groundCheckExtra,
            groundMask,
            QueryTriggerInteraction.Ignore
        );
    }


    public void SetDive(bool diving)
    {
        isDiving = diving;
    }

    public void SetGroundPound(bool active)
    {
        isGroundPounding = active;
    }

    public void SetCarrying(bool carrying)
    {
        isCarrying = carrying;
    }


    public bool IsGroundPounding()
    {
        return isGroundPounding;
    }

    public void PlayGemPose()
    {
        isGemPose = true;
    }

    public void StopGemPose()
    {
        isGemPose = false;
    }


    void Update()
    {
        float gemTarget = isGemPose ? 1f : 0f;
        gemBlend = Mathf.MoveTowards(
            gemBlend,
            gemTarget,
            Time.deltaTime * gemBlendSpeed
        );


        bool grounded = IsGrounded();

        float horizontalSpeed = externalMoveBlend;


        bool shouldIdle = grounded && horizontalSpeed < 0.1f && !isDiving;


        float idleTarget = shouldIdle ? 1f : 0f;

        idleBlend = Mathf.MoveTowards(idleBlend, idleTarget, Time.deltaTime * idleBlendSpeed);

        float diveTarget = isDiving ? 1f : 0f;
        diveBlend = Mathf.MoveTowards(
            diveBlend,
            diveTarget,
            Time.deltaTime * diveBlendSpeed
        );

        float poundTarget = isGroundPounding ? 1f : 0f;
        poundBlend = Mathf.MoveTowards(
            poundBlend,
            poundTarget,
            Time.deltaTime * poundBlendSpeed
        );

        // Smooth blend
        float target = grounded ? 0f : 1f;
        jumpBlend = Mathf.MoveTowards(jumpBlend, target, Time.deltaTime * jumpBlendSpeed);

        float carryTarget = isCarrying ? 1f : 0f;
        carryBlend = Mathf.MoveTowards(carryBlend, carryTarget, Time.deltaTime * carryBlendSpeed);

        AnimateWalk();

        if (!isGroundPounding)
        {
            AnimateJumpPose(jumpBlend);
        }


        AnimateDivePose(diveBlend);

        AnimateGroundPound(poundBlend);

        AnimateCarryPose(carryBlend);

        AnimateIdle(idleBlend);

        AnimateGemPose(gemBlend);

    }

    void AnimateWalk()
    {
        if (isGemPose) return;

        // Use hub-provided movement amount (0..1), not rigidbody velocity
        float speed01 = externalMoveBlend;
        if (speed01 < 0.1f) return;

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

        if (isGroundPounding)
            return;
    }


    void AnimateJumpPose(float blend)
    {
        if (isGemPose) return;


        if (isGroundPounding)
            return;

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

    void AnimateDivePose(float blend)
    {
        if (isGemPose) return;


        if (isGroundPounding)
            return;

        if (blend <= 0f) return;

        // Body pitch down
        body.localRotation = Quaternion.Lerp(
            body.localRotation,
            Quaternion.Euler(-diveBodyPitch, 0, 0),
            blend
        );

        // Arms forward
        leftArm.localRotation = Quaternion.Lerp(
            leftArm.localRotation,
            leftArmStartRot * Quaternion.Euler(diveArmForwardAngle, 0, 0),
            blend
        );

        rightArm.localRotation = Quaternion.Lerp(
            rightArm.localRotation,
            rightArmStartRot * Quaternion.Euler(diveArmForwardAngle, 0, 0),
            blend
        );

        // Legs back
        leftLeg.localRotation = Quaternion.Lerp(
            leftLeg.localRotation,
            leftLegStartRot * Quaternion.Euler(-diveLegBackAngle, 0, 0),
            blend
        );

        rightLeg.localRotation = Quaternion.Lerp(
            rightLeg.localRotation,
            rightLegStartRot * Quaternion.Euler(-diveLegBackAngle, 0, 0),
            blend
        );
    }

    void AnimateGroundPound(float blend)
    {
        if (isGemPose) return;


        if (blend <= 0f) return;

        // Body lean forward
        body.localRotation = Quaternion.Lerp(
            body.localRotation,
            Quaternion.Euler(poundBodyLean, 0, 0),
            blend
        );

        // Arms straight down
        leftArm.localRotation = Quaternion.Lerp(
            leftArm.localRotation,
            leftArmStartRot * Quaternion.Euler(poundArmDownAngle, 0, 0),
            blend
        );

        rightArm.localRotation = Quaternion.Lerp(
            rightArm.localRotation,
            rightArmStartRot * Quaternion.Euler(poundArmDownAngle, 0, 0),
            blend
        );

        // Legs straight / stiff
        leftLeg.localRotation = Quaternion.Lerp(
            leftLeg.localRotation,
            leftLegStartRot * Quaternion.Euler(poundLegStraightAngle, 0, 0),
            blend
        );

        rightLeg.localRotation = Quaternion.Lerp(
            rightLeg.localRotation,
            rightLegStartRot * Quaternion.Euler(poundLegStraightAngle, 0, 0),
            blend
        );
    }


    void AnimateIdle(float blend)
    {
        if (isGemPose) return;


        if (isGroundPounding)
            return;

        if (blend <= 0f) return;

        // Body breathe ALWAYS (even while carrying)
        float breathe = Mathf.Sin(Time.time * 2f) * 0.05f;
        body.localPosition = bodyStartPos + Vector3.up * breathe;

        // Legs straight ALWAYS (even while carrying)
        leftLeg.localRotation = Quaternion.Lerp(
            leftLeg.localRotation,
            leftLegStartRot,
            blend
        );

        rightLeg.localRotation = Quaternion.Lerp(
            rightLeg.localRotation,
            rightLegStartRot,
            blend
        );

        // Arms only if NOT carrying
        if (isCarrying)
            return;

        float t = Time.time * idleArmBobSpeed;
        float armBob = Mathf.Sin(t) * idleArmBobAmount;

        leftArm.localRotation = Quaternion.Lerp(
            leftArm.localRotation,
            leftArmStartRot * Quaternion.Euler(armBob, 0, 0),
            blend
        );

        rightArm.localRotation = Quaternion.Lerp(
            rightArm.localRotation,
            rightArmStartRot * Quaternion.Euler(armBob, 0, 0),
            blend
        );
    }



    void AnimateCarryPose(float blend)
    {
        if (blend <= 0f) return;
        if (isGroundPounding) return; // optional: ground pound overrides carry

        // Arms forward like holding something
        Quaternion leftCarry =
            leftArmStartRot * Quaternion.Euler(carryArmForwardAngle, -carryArmOutAngle, 0);

        Quaternion rightCarry =
            rightArmStartRot * Quaternion.Euler(carryArmForwardAngle, carryArmOutAngle, 0);

        leftArm.localRotation = Quaternion.Lerp(leftArm.localRotation, leftCarry, blend);
        rightArm.localRotation = Quaternion.Lerp(rightArm.localRotation, rightCarry, blend);
    }

    void AnimateGemPose(float blend)
    {
        if (blend <= 0f) return;

        // Body slight lean back (celebratory)
        body.localRotation = Quaternion.Lerp(
            body.localRotation,
            Quaternion.Euler(gemBodyLean, 0, 0),
            blend
        );

        // Arms straight up
        leftArm.localRotation = Quaternion.Lerp(
            leftArm.localRotation,
            leftArmStartRot * Quaternion.Euler(gemArmUpAngle, 0, 0),
            blend
        );

        rightArm.localRotation = Quaternion.Lerp(
            rightArm.localRotation,
            rightArmStartRot * Quaternion.Euler(gemArmUpAngle, 0, 0),
            blend
        );

        // Legs stay neutral
        leftLeg.localRotation = Quaternion.Lerp(
            leftLeg.localRotation,
            leftLegStartRot,
            blend
        );

        rightLeg.localRotation = Quaternion.Lerp(
            rightLeg.localRotation,
            rightLegStartRot,
            blend
        );
    }

}
