using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PressurePlatform : MonoBehaviour
{
    public Transform pointUp;
    public Transform pointDown;
    public float speed = 4f;
    public AudioSource blockSFX;

    Rigidbody rb;
    int playersOnTop;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    void FixedUpdate()
    {
        bool pressed = playersOnTop > 0;

        Vector3 target = pressed ? pointDown.position : pointUp.position;

        Vector3 newPos = Vector3.MoveTowards(
            rb.position,
            target,
            speed * Time.fixedDeltaTime
        );

        rb.MovePosition(newPos);
        
    }

    // Called by trigger child
    public void PlayerEnter()
    {
        playersOnTop++;
        blockSFX.Play();
    }

    public void PlayerExit()
    {
        playersOnTop = Mathf.Max(0, playersOnTop - 1);
        blockSFX.Play();
    }
}
