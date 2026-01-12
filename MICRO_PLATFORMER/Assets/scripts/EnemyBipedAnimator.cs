using UnityEngine;

public class EnemyBipedAnimator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Transform body;
    [SerializeField] Transform leftArm;
    [SerializeField] Transform rightArm;
    [SerializeField] Transform leftLeg;
    [SerializeField] Transform rightLeg;

    [Header("Walk")]
    [SerializeField] float armSwing = 25f;
    [SerializeField] float legSwing = 35f;
    [SerializeField] float walkSpeed = 7f;
    [SerializeField] float bodyBob = 0.07f;

    [Header("Throw")]
    [SerializeField] float throwBackAngle = 60f;
    [SerializeField] float throwSpeed = 10f;
    [SerializeField] float throwForwardAngle = -70f;
    

    bool isShooting;
    float throwBlend;


    Rigidbody rb;
    Vector3 bodyStartPos;

    Quaternion laStart, raStart, llStart, rlStart;


    public bool IsShooting()
    {
        return isShooting;
    }

    public void PlayShoot()
    {
        if (isShooting) return; // prevent overlapping throws
        isShooting = true;
        throwBlend = 0f;
    }



    void Start()
    {
        rb = GetComponentInParent<Rigidbody>();

        bodyStartPos = body.localPosition;

        laStart = leftArm.localRotation;
        raStart = rightArm.localRotation;
        llStart = leftLeg.localRotation;
        rlStart = rightLeg.localRotation;
    }

    void Update()
    {
        if (isShooting)
        {
            AnimateThrow();
            return; // don't animate walk while throwing
        }

        AnimateWalk();
    }


    void AnimateWalk()
    {
        float speed = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z).magnitude;

        if (speed < 0.1f)
        {
            Idle();
            return;
        }

        float t = Time.time * walkSpeed;
        float arm = Mathf.Sin(t) * armSwing;
        float leg = Mathf.Sin(t) * legSwing;

        leftArm.localRotation = laStart * Quaternion.Euler(arm, 0, 0);
        rightArm.localRotation = raStart * Quaternion.Euler(-arm, 0, 0);

        leftLeg.localRotation = llStart * Quaternion.Euler(-leg, 0, 0);
        rightLeg.localRotation = rlStart * Quaternion.Euler(leg, 0, 0);

        body.localPosition =
            bodyStartPos + Vector3.up * Mathf.Abs(Mathf.Sin(t)) * bodyBob;
    }

    void Idle()
    {
        float breathe = Mathf.Sin(Time.time * 2f) * 0.04f;
        body.localPosition = bodyStartPos + Vector3.up * breathe;
    }

    void AnimateThrow()
    {
        throwBlend += Time.deltaTime * throwSpeed;

        float t = Mathf.Sin(throwBlend * Mathf.PI); // smooth forward + back

        // Use raStart (your rightArm starting rotation)
        Quaternion throwRot =
            raStart *
            Quaternion.Euler(Mathf.Lerp(throwBackAngle, throwForwardAngle, t), 0, 0);

        rightArm.localRotation = throwRot;

        if (throwBlend >= 1f)
        {
            isShooting = false;
            rightArm.localRotation = raStart; // reset
        }
    }





    
}
