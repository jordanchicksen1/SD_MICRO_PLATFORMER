using UnityEngine;

public class MenuEnemyAnimator : MonoBehaviour
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

    Quaternion flStart, frStart, blStart, brStart;
    Vector3 bodyStartPos;

    void Start()
    {
        bodyStartPos = body.localPosition;

        flStart = frontLeft.localRotation;
        frStart = frontRight.localRotation;
        blStart = backLeft.localRotation;
        brStart = backRight.localRotation;
    }

    void Update()
    {
        AnimateWalk();
    }

    void AnimateWalk()
    {
        float t = Time.time * walkSpeed;
        float swing = Mathf.Sin(t) * legSwing;

        // Diagonal pairing (same as gameplay animator)
        frontLeft.localRotation = flStart * Quaternion.Euler(swing, 0, 0);
        backRight.localRotation = brStart * Quaternion.Euler(swing, 0, 0);

        frontRight.localRotation = frStart * Quaternion.Euler(-swing, 0, 0);
        backLeft.localRotation = blStart * Quaternion.Euler(-swing, 0, 0);

        body.localPosition =
            bodyStartPos + Vector3.up * Mathf.Abs(Mathf.Sin(t)) * bodyBobAmount;
    }
}