using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Transform leftArm;
    [SerializeField] Transform rightArm;
    [SerializeField] Transform leftLeg;
    [SerializeField] Transform rightLeg;
    [SerializeField] Transform body;
    [SerializeField] Transform head;
    [SerializeField] Transform model;
    [SerializeField] Transform baseballBat;
    Vector3 headStartPos;
    Vector3 modelStartPos;
    Quaternion headStartRot;
    Vector3 rightLegStartPos;
    Vector3 leftLegStartPos;
    Vector3 rightArmStartPos;
    Vector3 leftArmStartPos;
    Quaternion leftArmStartRot;
    Quaternion rightArmStartRot;
    Quaternion leftLegStartRot;
    Quaternion rightLegStartRot;
    Quaternion modelStartRotation;
    Quaternion batStartRot;
    Vector3 batStartPos;

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

    [Header("Kick Pose")]
    [SerializeField] float kickLegForwardAngle = 85f;
    [SerializeField] float kickBodyLean = -12f;
    [SerializeField] float kickArmBackAngle = -25f;
    [SerializeField] float kickBlendSpeed = 12f;
    bool isKicking;
    float kickBlend;

    [Header("Bat Pose")]
    [SerializeField] float batSwingAngle = 140f;
    [SerializeField] float batWindupAngle = -55f;
    [SerializeField] float batBodyTurn = 25f;
    [SerializeField] float batBodyLean = -8f;
    [SerializeField] float batBlendSpeed = 14f;
    bool isBatSpin;
    float batSpinBlend;
    bool isBatWindup;
    bool isBatFollowThrough;
    float batWindupBlend;
    float batFollowBlend;

    [Header("Boomerang Pose")]
    [SerializeField] float boomerangBlendSpeed = 5f;
    bool isBoomerangWindup;
    bool isBoomerangThrow;

    float boomerangWindupBlend;
    float boomerangThrowBlend;

    [Header("Boxing Gloves Pose")]
    [SerializeField] float gloveBlendSpeed = 14f;
    bool isGloveWindup;
    bool isUppercut;
    bool isGroundSlamJump;
    bool isGroundSlamImpact;
    float gloveWindupBlend;
    float uppercutBlend;
    float groundSlamJumpBlend;
    float groundSlamImpactBlend;
    bool rightPunch;


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
        headStartPos = head.localPosition;
        headStartRot = head.localRotation;
        rightLegStartPos = rightLeg.localPosition;
        modelStartRotation = model.localRotation;
        batStartRot = baseballBat.localRotation;
        batStartPos = baseballBat.localPosition;
        rightArmStartPos = rightArm.localPosition;
        leftArmStartPos = leftArm.localPosition;
        leftLegStartPos = leftLeg.localPosition;
    }

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, 1.1f);
    }

    public void SetDive(bool diving)
    {
        isDiving = diving;
    }

    public void SetKick(bool kicking)
    {
        isKicking = kicking;
    }

    public void SetBatWindup(bool active)
    {
        isBatWindup = active;
    }

    public void SetBatFollowThrough(bool active)
    {
        isBatFollowThrough = active;
    }

    public void SetBatSpin(bool spinning)
    {
        isBatSpin = spinning;

        if (!spinning)
        {
            model.localRotation = modelStartRotation;
        }
    }

    public void SetBoomerangWindup(bool active)
    {
        isBoomerangWindup = active;
    }

    public void SetBoomerangThrow(bool active)
    {
        isBoomerangThrow = active;
    }

    public void SetGloveWindup(bool active)
    {
        isGloveWindup = active;
    }

    public void SetUppercut(bool active)
    {
        isUppercut = active;
    }

    public void SetRightPunch(bool value)
    {
        rightPunch = value;
    }

    public void SetGroundSlamJump(bool active)
    {
        isGroundSlamJump = active;
    }

    public void SetGroundSlamImpact(bool active)
    {
        isGroundSlamImpact = active;
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

        float kickTarget = isKicking ? 1f : 0f;
        kickBlend = Mathf.MoveTowards(
            kickBlend,
            kickTarget,
            Time.deltaTime * kickBlendSpeed);

        float windupTarget = isBatWindup ? 1f : 0f;

        batWindupBlend = Mathf.MoveTowards(
            batWindupBlend,
            windupTarget,
            Time.deltaTime * batBlendSpeed);

        float followTarget = isBatFollowThrough ? 1f : 0f;

        batFollowBlend = Mathf.MoveTowards(
            batFollowBlend,
            followTarget,
            Time.deltaTime * batBlendSpeed);

        float spinTarget = isBatSpin ? 1f : 0f;

        batSpinBlend = Mathf.MoveTowards(
            batSpinBlend,
            spinTarget,
            Time.deltaTime * batBlendSpeed);

        float boomerangWindupTarget = isBoomerangWindup ? 1f : 0f;

        boomerangWindupBlend = Mathf.MoveTowards(
            boomerangWindupBlend,
            boomerangWindupTarget,
            Time.deltaTime * boomerangBlendSpeed);


        float boomerangThrowTarget = isBoomerangThrow ? 1f : 0f;

        boomerangThrowBlend = Mathf.MoveTowards(
            boomerangThrowBlend,
            boomerangThrowTarget,
            Time.deltaTime * boomerangBlendSpeed);

        float gloveWindupTarget = isGloveWindup ? 1f : 0f;

        gloveWindupBlend = Mathf.MoveTowards(
            gloveWindupBlend,
            gloveWindupTarget,
            Time.deltaTime * gloveBlendSpeed);

        float uppercutTarget = isUppercut ? 1f : 0f;

        uppercutBlend = Mathf.MoveTowards(
            uppercutBlend,
            uppercutTarget,
            Time.deltaTime * gloveBlendSpeed);

        float slamJumpTarget = isGroundSlamJump ? 1f : 0f;

        groundSlamJumpBlend = Mathf.MoveTowards(
            groundSlamJumpBlend,
            slamJumpTarget,
            Time.deltaTime * gloveBlendSpeed);

        float slamImpactTarget = isGroundSlamImpact ? 1f : 0f;

        groundSlamImpactBlend = Mathf.MoveTowards(
            groundSlamImpactBlend,
            slamImpactTarget,
            Time.deltaTime * gloveBlendSpeed);

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

        AnimateKick(kickBlend);

        AnimateBatWindup(batWindupBlend);
        AnimateBatFollowThrough(batFollowBlend);
        AnimateBatSpin(batSpinBlend);


        AnimateBoomerangWindup(boomerangWindupBlend);
        AnimateBoomerangThrow(boomerangThrowBlend);

        AnimateGloveWindup(gloveWindupBlend);
        AnimateUppercut(uppercutBlend);
        AnimateGroundSlamJump(groundSlamJumpBlend);
        AnimateGroundSlamImpact(groundSlamImpactBlend);

        
    }

    void AnimateWalk()
    {
        if (isGemPose) return;

        if (isBatSpin)
            return;


        float speed = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z).magnitude;

        if (speed < 0.1f)
            return;

        float t = Time.time * walkSpeed;

        float armSwing = Mathf.Sin(t) * armSwingAmount;
        float legSwing = Mathf.Sin(t) * legSwingAmount;

        if (!isCarrying) 
        {
            // Arms (slightly softer)
            leftArm.localRotation = leftArmStartRot * Quaternion.Euler(armSwing, 0, 0);
            rightArm.localRotation = rightArmStartRot * Quaternion.Euler(-armSwing, 0, 0);
        }

       
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

        if (isBatSpin)
            return;

        if (isGroundPounding)
            return;

        if (IsAnyWeaponAnimationActive())
            return;

        // Body lean
        body.localRotation = Quaternion.Lerp(
            Quaternion.identity,
            Quaternion.Euler(jumpLean, 0, 0),
            blend
        );


        if (!isCarrying)
        {
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

    void AnimateDivePose(float blend)
    {
        if (isGemPose) return;

        if (isBatSpin)
            return;

        if (isGroundPounding)
            return;

        if (IsAnyWeaponAnimationActive())
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

        if (isBatSpin)
            return;

        if (IsAnyWeaponAnimationActive())
            return;

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

        if (isBatSpin)
            return;

        if (isGroundPounding)
            return;

        if (IsAnyWeaponAnimationActive())
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
        if (isBatSpin)
            return;

        if (blend <= 0f) return;
        if (isGroundPounding) return;

        if (IsAnyWeaponAnimationActive())
            return;

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

        if (isBatSpin)
            return;

        if (IsAnyWeaponAnimationActive())
            return;

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

    bool IsAnyWeaponAnimationActive()
    {
        return isBatWindup ||
               isBatFollowThrough ||
               isBatSpin ||
               isBoomerangWindup ||
               isBoomerangThrow ||
               isGloveWindup ||
               isUppercut ||
               isGroundSlamJump ||
               isGroundSlamImpact;
    }

    void AnimateKick(float blend)
    {
        if (isGemPose) return;

        if (isBatSpin)
            return;

        if (blend <= 0f)
        {
            head.localPosition = Vector3.Lerp(
                head.localPosition,
                headStartPos,
                Time.deltaTime * 12f);

            rightLeg.localPosition = Vector3.Lerp(
                rightLeg.localPosition,
                rightLegStartPos,
                Time.deltaTime * 12f);

            return;
        }

        body.localRotation = Quaternion.Lerp(
            body.localRotation,
            Quaternion.Euler(kickBodyLean, 0, 0),
            blend);

        head.localPosition = Vector3.Lerp(
            head.localPosition,
            headStartPos + new Vector3(0f, 0.05f, 0.08f),
            blend);

        rightLeg.localRotation = Quaternion.Lerp(
            rightLeg.localRotation,
            rightLegStartRot * Quaternion.Euler(-kickLegForwardAngle, 0, 0),
            blend);

        rightLeg.localPosition = Vector3.Lerp(
          rightLeg.localPosition,
          rightLegStartPos + new Vector3(0f, 0.1f, -0.35f),
          blend);

        leftArm.localRotation = Quaternion.Lerp(
            leftArm.localRotation,
            leftArmStartRot * Quaternion.Euler(kickArmBackAngle, 0, 0),
            blend);

        rightArm.localRotation = Quaternion.Lerp(
            rightArm.localRotation,
            rightArmStartRot * Quaternion.Euler(kickArmBackAngle, 0, 0),
            blend);
    }

    void AnimateBatWindup(float blend)
    {
       

        if (isGemPose) return;

        if (isBatSpin)
            return;

        if (blend <= 0f)
            return;

        // Body twists into the swing
        body.localRotation = Quaternion.Lerp(
            body.localRotation,
            Quaternion.Euler(batBodyLean, batBodyTurn, 0f),
            blend);

        // Right arm swings the bat
        rightArm.localRotation = Quaternion.Lerp(
    rightArm.localRotation,
    rightArmStartRot * Quaternion.Euler(
        120f,
        45f,
        -15f),
    blend);

     

        // Head leans slightly into the hit
        head.localPosition = Vector3.Lerp(
            head.localPosition,
            headStartPos + new Vector3(0f, 0.03f, 0.06f),
            blend);
    }

    void AnimateBatFollowThrough(float blend)
    {
      

        if (isGemPose) return;

        if (isBatSpin)
            return;

        if (blend <= 0f)
            return;

        // Twist body the opposite direction
        body.localRotation = Quaternion.Lerp(
            body.localRotation,
            Quaternion.Euler(-6f, -25f, 0f),
            blend);

        // Right arm swings across the player
        rightArm.localRotation = Quaternion.Lerp(
            rightArm.localRotation,
            rightArmStartRot * Quaternion.Euler(
             10f,
             -75f,
             -15f),
            blend);

        // Head follows the swing
        head.localPosition = Vector3.Lerp(
            head.localPosition,
            headStartPos + new Vector3(0f, 0.02f, -0.04f),
            blend);
    }


    void AnimateBatSpin(float blend)
    {
        if (isGemPose) return;

        if (isCarrying)
            return;

        if (blend <= 0f)
        {
            model.localRotation = Quaternion.Lerp(
                model.localRotation,
                modelStartRotation,
                Time.deltaTime * 15f);

            baseballBat.localRotation = Quaternion.Lerp(
                baseballBat.localRotation,
                batStartRot,
                Time.deltaTime * 15f);

            baseballBat.localPosition = Vector3.Lerp(
                baseballBat.localPosition,
                batStartPos,
                Time.deltaTime * 15f);

            leftArm.localPosition = Vector3.Lerp(
    leftArm.localPosition,
    leftArmStartPos,
    Time.deltaTime * 15f);

            leftArm.localRotation = Quaternion.Lerp(
                leftArm.localRotation,
                leftArmStartRot,
                Time.deltaTime * 15f);

            rightArm.localPosition = Vector3.Lerp(
    rightArm.localPosition,
    rightArmStartPos,
    Time.deltaTime * 15f);

            rightArm.localRotation = Quaternion.Lerp(
                rightArm.localRotation,
                rightArmStartRot,
                Time.deltaTime * 15f);

            return;
        }

        model.Rotate(
                Vector3.up,
                900f * Time.deltaTime,
                Space.Self);

        body.localRotation = Quaternion.Lerp(
            body.localRotation,
            Quaternion.Euler(0f, 0f, 0f),
            blend);

        leftArm.localPosition = Vector3.Lerp(
    leftArm.localPosition,
    new Vector3(
        0.347000003f,
        0.273999989f,
        -0.337000012f),
    blend);

        leftArm.localRotation = Quaternion.Lerp(
            leftArm.localRotation,
            Quaternion.Euler(
                0f,
                105.748154f,
                0f),
            blend);

        rightArm.localPosition = Vector3.Lerp(
    rightArm.localPosition,
    new Vector3(
        -0.360000014f,
        0.307999998f,
        -0.326000005f),
    blend);

        rightArm.localRotation = Quaternion.Lerp(
            rightArm.localRotation,
            Quaternion.Euler(
                0f,
                252.36673f,
                0f),
            blend);

        baseballBat.localPosition = Vector3.Lerp(
    baseballBat.localPosition,
    new Vector3(
        -2.1500001f,
        -2.8599999f,
        0.189999998f),
    blend);

        baseballBat.localRotation = Quaternion.Lerp(
            baseballBat.localRotation,
            Quaternion.Euler(
                42.6191826f,
                293.049744f,
                196.07251f),
            blend);

    }

    void AnimateBoomerangWindup(float blend)
    {
        if (blend <= 0f)
            return;

        leftArm.localPosition = Vector3.Lerp(
            leftArm.localPosition,
            new Vector3(
                -0.018f,
                0.274f,
                -0.416f),
            blend);

        rightArm.localPosition = Vector3.Lerp(
            rightArm.localPosition,
            new Vector3(
                -0.005f,
                0.296f,
                0.416f),
            blend);

        leftArm.localRotation = Quaternion.Lerp(
            leftArm.localRotation,
            Quaternion.Euler(
                0f,
                90f,
                45f),
            blend);

        rightArm.localRotation = Quaternion.Lerp(
            rightArm.localRotation,
            Quaternion.Euler(
                0f,
                90f,
                -45f),
            blend);
    }

    void AnimateBoomerangThrow(float blend)
    {
        if (blend <= 0f)
            return;

        leftArm.localPosition = Vector3.Lerp(
            leftArm.localPosition,
            new Vector3(
                -0.005f,
                0.296f,
                0.416f),
            blend);

        rightArm.localPosition = Vector3.Lerp(
            rightArm.localPosition,
            new Vector3(
                -0.018f,
                0.274f,
                -0.416f),
            blend);

        leftArm.localRotation = Quaternion.Lerp(
            leftArm.localRotation,
            Quaternion.Euler(
                0f,
                -90f,
                45f),
            blend);

        rightArm.localRotation = Quaternion.Lerp(
            rightArm.localRotation,
            Quaternion.Euler(
                0f,
                -90f,
                -45f),
            blend);
    }

    void AnimateGloveWindup(float blend)
    {
        if (isGemPose) return;

        if (isBatSpin)
            return;

        if (blend <= 0f)
            return;

        if (rightPunch)
        {
            // Right arm (punch arm)
            rightArm.localPosition = Vector3.Lerp(
                rightArm.localPosition,
                new Vector3(
                    -0.556999981f,
                    0.25184235f,
                    0.0480000004f),
                blend);

            rightArm.localRotation = Quaternion.Lerp(
                rightArm.localRotation,
                new Quaternion(
                    -0.0937879831f,
                     0.290423512f,
                    -0.0286157709f,
                     0.951860845f),
                blend);

            // Left arm (guard)
            leftArm.localPosition = Vector3.Lerp(
                leftArm.localPosition,
                new Vector3(
                     0.58379674f,
                     0.23299998f,
                     0.0410000011f),
                blend);

            leftArm.localRotation = Quaternion.Lerp(
                leftArm.localRotation,
                new Quaternion(
                     0.298097938f,
                     0.22401832f,
                    -0.0721889734f,
                     0.925063372f),
                blend);
        }
        else
        {
            // Left arm (punch arm)
            leftArm.localPosition = Vector3.Lerp(
                leftArm.localPosition,
                new Vector3(
                     0.538999975f,
                     0.169814348f,
                    -0.0314626694f),
                blend);

            leftArm.localRotation = Quaternion.Lerp(
                leftArm.localRotation,
                new Quaternion(
                    -0.3253313f,
                    -0.0734869763f,
                    -0.0253688842f,
                     0.942398906f),
                blend);

            // Right arm (guard)
            rightArm.localPosition = Vector3.Lerp(
                rightArm.localPosition,
                new Vector3(
                    -0.583000004f,
                     0.271535039f,
                     0.142667294f),
                blend);

            rightArm.localRotation = Quaternion.Lerp(
                rightArm.localRotation,
                new Quaternion(
                     0.201587901f,
                    -0.178198114f,
                     0.0373260193f,
                     0.962400496f),
                blend);
        }
    }

    void AnimateUppercut(float blend)
    {
        if (isGemPose) return;

        if (isBatSpin)
            return;

        if (blend <= 0f)
        {
            leftArm.localPosition = Vector3.Lerp(
                leftArm.localPosition,
                leftArmStartPos,
                Time.deltaTime * 15f);

            rightArm.localPosition = Vector3.Lerp(
                rightArm.localPosition,
                rightArmStartPos,
                Time.deltaTime * 15f);

            leftArm.localRotation = Quaternion.Lerp(
                leftArm.localRotation,
                leftArmStartRot,
                Time.deltaTime * 15f);

            rightArm.localRotation = Quaternion.Lerp(
                rightArm.localRotation,
                rightArmStartRot,
                Time.deltaTime * 15f);

            return;
        }

        if (rightPunch)
        {
            // Right arm uppercut
            rightArm.localPosition = Vector3.Lerp(
                rightArm.localPosition,
                new Vector3(-0.537999988f, 0.250999987f, -0.222000003f),
                blend);

            rightArm.localRotation = Quaternion.Lerp(
                rightArm.localRotation,
                new Quaternion(0.396975577f, -0.637150705f, -0.41621387f, 0.513045251f),
                blend);

            // Left arm guard
            leftArm.localPosition = Vector3.Lerp(
                leftArm.localPosition,
                new Vector3(
                     0.533110857f,
                     0.270000011f,
                     0.108999997f),
                blend);

            leftArm.localRotation = Quaternion.Lerp(
                leftArm.localRotation,
                new Quaternion(
                    -0.107257284f,
                    -0.288246751f,
                     0.00292336312f,
                     0.951525688f),
                blend);
        }
        else
        {
            // Left arm uppercut
            leftArm.localPosition = Vector3.Lerp(
                leftArm.localPosition,
                new Vector3(0.456f, 0.273000002f, -0.293000013f),
                blend);

            leftArm.localRotation = Quaternion.Lerp(
                leftArm.localRotation,
                new Quaternion(0.418233216f, 0.580632448f, 0.469551772f, 0.517173171f),
                blend);

            // Right arm guard
            rightArm.localPosition = Vector3.Lerp(
                rightArm.localPosition,
                new Vector3(
                    -0.563000023f,
                     0.254999995f,
                     0.0799999982f),
                blend);

            rightArm.localRotation = Quaternion.Lerp(
                rightArm.localRotation,
                new Quaternion(
                    -0.130079657f,
                     0.257350147f,
                    -0.0349844657f,
                     0.956883669f),
                blend);
        }
    }

    void AnimateGroundSlamJump(float blend)
    {
        if (isGemPose) return;

        if (isBatSpin)
            return;

        if (blend <= 0f)
        {
            if (isGloveWindup ||
       isUppercut ||
       isGroundSlamImpact)
                return;


            model.localPosition = Vector3.Lerp(model.localPosition, Vector3.zero, Time.deltaTime * 15f);
            model.localRotation = Quaternion.Lerp(model.localRotation, modelStartRotation, Time.deltaTime * 15f);

            head.localPosition = Vector3.Lerp(head.localPosition, headStartPos, Time.deltaTime * 15f);
            head.localRotation = Quaternion.Lerp(head.localRotation, headStartRot, Time.deltaTime * 15f);

            leftArm.localPosition = Vector3.Lerp(leftArm.localPosition, leftArmStartPos, Time.deltaTime * 15f);
            leftArm.localRotation = Quaternion.Lerp(leftArm.localRotation, leftArmStartRot, Time.deltaTime * 15f);

            rightArm.localPosition = Vector3.Lerp(rightArm.localPosition, rightArmStartPos, Time.deltaTime * 15f);
            rightArm.localRotation = Quaternion.Lerp(rightArm.localRotation, rightArmStartRot, Time.deltaTime * 15f);

            leftLeg.localPosition = Vector3.Lerp(leftLeg.localPosition, leftLegStartPos, Time.deltaTime * 15f);
            leftLeg.localRotation = Quaternion.Lerp(leftLeg.localRotation, leftLegStartRot, Time.deltaTime * 15f);

            rightLeg.localPosition = Vector3.Lerp(rightLeg.localPosition, rightLegStartPos, Time.deltaTime * 15f);
            rightLeg.localRotation = Quaternion.Lerp(rightLeg.localRotation, rightLegStartRot, Time.deltaTime * 15f);

            return;
        }

        head.localPosition = Vector3.Lerp(
            head.localPosition,
            new Vector3(0f, 0.560222983f, -0.00424337387f),
            blend);

        head.localRotation = Quaternion.Lerp(
            head.localRotation,
            new Quaternion(0.151070535f, 0f, 0f, 0.988523006f),
            blend);

        leftArm.localPosition = Vector3.Lerp(
            leftArm.localPosition,
            new Vector3(0.402666807f, 0.236000001f, 0.100000001f),
            blend);

        leftArm.localRotation = Quaternion.Lerp(
            leftArm.localRotation,
            new Quaternion(-0.139028266f, -0.178253889f, 0.599084675f, 0.768110871f),
            blend);

        rightArm.localPosition = Vector3.Lerp(
            rightArm.localPosition,
            new Vector3(-0.372319162f, 0.287999988f, 0.0489999987f),
            blend);

        rightArm.localRotation = Quaternion.Lerp(
            rightArm.localRotation,
            new Quaternion(-0.106027693f, 0.136001959f, -0.605626822f, 0.776838303f),
            blend);

        leftLeg.localPosition = Vector3.Lerp(
            leftLeg.localPosition,
            new Vector3(0.280000001f, -0.324843466f, 0.0719999969f),
            blend);

        leftLeg.localRotation = Quaternion.Lerp(
            leftLeg.localRotation,
            new Quaternion(-0.268155158f, 0f, 0f, 0.963375747f),
            blend);

        rightLeg.localPosition = Vector3.Lerp(
            rightLeg.localPosition,
            new Vector3(-0.280000001f, -0.352935255f, 0.119999997f),
            blend);

        rightLeg.localRotation = Quaternion.Lerp(
            rightLeg.localRotation,
            new Quaternion(-0.235904902f, 0f, 0f, 0.971776128f),
            blend);
    }

    void AnimateGroundSlamImpact(float blend)
    {
        if (isGemPose) return;

        if (isBatSpin)
            return;

        if (blend <= 0f)
        {
            if (isGloveWindup ||
        isUppercut ||
        isGroundSlamJump)
                return;


            model.localPosition = Vector3.Lerp(model.localPosition, Vector3.zero, Time.deltaTime * 15f);
            model.localRotation = Quaternion.Lerp(model.localRotation, modelStartRotation, Time.deltaTime * 15f);

            head.localPosition = Vector3.Lerp(head.localPosition, headStartPos, Time.deltaTime * 15f);
            head.localRotation = Quaternion.Lerp(head.localRotation, headStartRot, Time.deltaTime * 15f);

            leftArm.localPosition = Vector3.Lerp(leftArm.localPosition, leftArmStartPos, Time.deltaTime * 15f);
            leftArm.localRotation = Quaternion.Lerp(leftArm.localRotation, leftArmStartRot, Time.deltaTime * 15f);

            rightArm.localPosition = Vector3.Lerp(rightArm.localPosition, rightArmStartPos, Time.deltaTime * 15f);
            rightArm.localRotation = Quaternion.Lerp(rightArm.localRotation, rightArmStartRot, Time.deltaTime * 15f);

            leftLeg.localPosition = Vector3.Lerp(leftLeg.localPosition, leftLegStartPos, Time.deltaTime * 15f);
            leftLeg.localRotation = Quaternion.Lerp(leftLeg.localRotation, leftLegStartRot, Time.deltaTime * 15f);

            rightLeg.localPosition = Vector3.Lerp(rightLeg.localPosition, rightLegStartPos, Time.deltaTime * 15f);
            rightLeg.localRotation = Quaternion.Lerp(rightLeg.localRotation, rightLegStartRot, Time.deltaTime * 15f);

            return;
        }

        model.localPosition = Vector3.Lerp(
            model.localPosition,
            new Vector3(0f, -0.0748822689f, 0.209164143f),
            blend);

        model.localRotation = Quaternion.Lerp(
            model.localRotation,
            new Quaternion(-0.712179244f, 0f, 0f, 0.701997697f),
            blend);

        head.localPosition = Vector3.Lerp(
            head.localPosition,
            new Vector3(0f, 0.54428792f, 0.146905661f),
            blend);

        head.localRotation = Quaternion.Lerp(
            head.localRotation,
            new Quaternion(0.284923673f, 0f, 0f, 0.958550274f),
            blend);

        leftArm.localPosition = Vector3.Lerp(
            leftArm.localPosition,
            new Vector3(0.418938845f, 0.377999991f, -0.196999997f),
            blend);

        leftArm.localRotation = Quaternion.Lerp(
            leftArm.localRotation,
            new Quaternion(0.309688658f, 0.183409095f, 0.59752667f, 0.716530621f),
            blend);

        rightArm.localPosition = Vector3.Lerp(
            rightArm.localPosition,
            new Vector3(-0.355157316f, 0.291999996f, -0.238999993f),
            blend);

        rightArm.localRotation = Quaternion.Lerp(
            rightArm.localRotation,
            new Quaternion(0.12802209f, -0.162709579f, -0.605677366f, 0.768303931f),
            blend);

        leftLeg.localPosition = Vector3.Lerp(
            leftLeg.localPosition,
            new Vector3(0.280000001f, -0.310000002f, -0.199000001f),
            blend);

        leftLeg.localRotation = Quaternion.Lerp(
            leftLeg.localRotation,
            new Quaternion(0.245990425f, 0f, 0f, 0.969272316f),
            blend);

        rightLeg.localPosition = Vector3.Lerp(
            rightLeg.localPosition,
            new Vector3(-0.280000001f, -0.351000011f, -0.194000006f),
            blend);

        rightLeg.localRotation = Quaternion.Lerp(
            rightLeg.localRotation,
            new Quaternion(0.262517631f, 0f, 0f, 0.964927256f),
            blend);
    }

}

