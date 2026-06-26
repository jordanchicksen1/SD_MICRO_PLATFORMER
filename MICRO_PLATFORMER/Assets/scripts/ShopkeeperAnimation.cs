using UnityEngine;

public class ShopkeeperAnimation : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Transform head;
    [SerializeField] Transform body;
    [SerializeField] Transform leftArm;
    [SerializeField] Transform rightArm;

    Quaternion headStartRot;
    Quaternion bodyStartRot;
    Quaternion leftArmStartRot;
    Quaternion rightArmStartRot;

    Vector3 bodyStartPos;

    [Header("Idle")]
    [SerializeField] float breatheHeight = 0.03f;
    [SerializeField] float breatheSpeed = 2f;

    [Header("Talking")]
    [SerializeField] float talkBobAmount = 8f;
    [SerializeField] float talkBobSpeed = 8f;

    [Header("No Money")]
    [SerializeField] float shakeAmount = 18f;
    [SerializeField] float shakeSpeed = 10f;

    [Header("Celebrate")]
    [SerializeField] float armRaiseAngle = -90f;
    [SerializeField] float celebrateBlendSpeed = 8f;

    bool isTalking;
    bool isShaking;
    bool isCelebrating;

    float celebrateBlend;

    void Start()
    {
        bodyStartPos = body.localPosition;

        headStartRot = head.localRotation;
        bodyStartRot = body.localRotation;

        leftArmStartRot = leftArm.localRotation;
        rightArmStartRot = rightArm.localRotation;
    }

    void Update()
    {
        AnimateIdle();

        if (isTalking)
            AnimateTalking();

        if (isShaking)
            AnimateHeadShake();

        float targetCelebrate = isCelebrating ? 1f : 0f;

        celebrateBlend = Mathf.MoveTowards(
            celebrateBlend,
            targetCelebrate,
            Time.deltaTime * celebrateBlendSpeed);

        AnimateCelebrate();
    }

    void AnimateIdle()
    {
        float breathe =
            Mathf.Sin(Time.time * breatheSpeed) * breatheHeight;

        body.localPosition =
            bodyStartPos + Vector3.up * breathe;
    }

    void AnimateTalking()
    {
        float bob =
            Mathf.Sin(Time.time * talkBobSpeed) * talkBobAmount;

        head.localRotation =
            headStartRot * Quaternion.Euler(bob, 0f, 0f);
    }

    void AnimateHeadShake()
    {
        float shake =
            Mathf.Sin(Time.time * shakeSpeed) * shakeAmount;

        head.localRotation =
            headStartRot * Quaternion.Euler(0f, shake, 0f);
    }

    void AnimateCelebrate()
    {
        Quaternion leftTarget =
            leftArmStartRot *
            Quaternion.Euler(armRaiseAngle, 0f, 0f);

        Quaternion rightTarget =
            rightArmStartRot *
            Quaternion.Euler(armRaiseAngle, 0f, 0f);

        leftArm.localRotation =
     Quaternion.Slerp(
         leftArmStartRot,
         leftTarget,
         celebrateBlend);

        rightArm.localRotation =
            Quaternion.Slerp(
                rightArmStartRot,
                rightTarget,
                celebrateBlend);
    }

    public void StartTalking()
    {
        isTalking = true;
        isShaking = false;
        isCelebrating = false;
    }

    public void StartHeadShake()
    {
        isTalking = false;
        isShaking = true;
        isCelebrating = false;
    }

    public void StartCelebrate()
    {
        isTalking = false;
        isShaking = false;
        isCelebrating = true;
    }

    public void StopAnimation()
    {
        isTalking = false;
        isShaking = false;
        isCelebrating = false;

        celebrateBlend = 0f;

        head.localRotation = headStartRot;
        body.localRotation = bodyStartRot;

    }
}