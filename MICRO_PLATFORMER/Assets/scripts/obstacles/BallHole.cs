using UnityEngine;
using UnityEngine.Events;

public class BallHole : MonoBehaviour
{
    [SerializeField] Transform snapPoint;
    [SerializeField] UnityEvent onBallPlaced;
    public AudioSource buttonSFX;

    bool filled;

    void Reset()
    {
        Transform t = transform.Find("SnapPoint");
        if (t != null) snapPoint = t;
    }

    void OnTriggerEnter(Collider other)
    {
        if (filled) return;

        CarryBall ball = other.GetComponentInParent<CarryBall>();
        if (ball == null) return;
        if (ball.IsLockedInHole) return;

        // Important: require it to be DROPPED (not still held)
        if (ball.IsHeld) return;

        filled = true;

        if (snapPoint == null) snapPoint = transform;

        ball.LockIntoHole(snapPoint);

        onBallPlaced?.Invoke();

        buttonSFX.Play();
    }
}
