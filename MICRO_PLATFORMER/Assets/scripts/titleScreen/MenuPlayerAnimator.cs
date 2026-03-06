using UnityEngine;

public class MenuPlayerAnimator : MonoBehaviour
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

    Vector3 bodyStartPos;

    [Header("Walk Animation")]
    [SerializeField] float armSwingAmount = 30f;
    [SerializeField] float legSwingAmount = 50f;
    [SerializeField] float walkSpeed = 8f;
    [SerializeField] float bodyBobAmount = 0.1f;
    [SerializeField] float bodyBobSpeed = 6f;

    void Start()
    {
        bodyStartPos = body.localPosition;

        leftArmStartRot = leftArm.localRotation;
        rightArmStartRot = rightArm.localRotation;
        leftLegStartRot = leftLeg.localRotation;
        rightLegStartRot = rightLeg.localRotation;
    }

    void Update()
    {
        AnimateWalk();
    }

    void AnimateWalk()
    {
        float t = Time.time * walkSpeed;

        float armSwing = Mathf.Sin(t) * armSwingAmount;
        float legSwing = Mathf.Sin(t) * legSwingAmount;

        // Arms
        leftArm.localRotation = leftArmStartRot * Quaternion.Euler(armSwing, 0, 0);
        rightArm.localRotation = rightArmStartRot * Quaternion.Euler(-armSwing, 0, 0);

        // Legs
        leftLeg.localRotation = leftLegStartRot * Quaternion.Euler(-legSwing, 0, 0);
        rightLeg.localRotation = rightLegStartRot * Quaternion.Euler(legSwing, 0, 0);

        // Body bob
        body.localPosition =
            bodyStartPos + Vector3.up * Mathf.Sin(t * bodyBobSpeed) * bodyBobAmount;
    }
}