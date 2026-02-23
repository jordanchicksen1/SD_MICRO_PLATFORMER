using UnityEngine;

public class BallObjectiveArrow : MonoBehaviour
{
    [SerializeField] CarryBall ball;
    [SerializeField] BallHole hole;
    [SerializeField] GameObject arrowVisual; // the arrow object above the hole

    [SerializeField] float bobHeight = 0.2f;
    [SerializeField] float bobSpeed = 2f;

    Vector3 startLocalPos;

    void Awake()
    {
        startLocalPos = arrowVisual.transform.localPosition;
    }


    void Start()
    {
        if (arrowVisual)
            arrowVisual.SetActive(false);
    }

    void Update()
    {
        if (!ball || !hole || !arrowVisual) return;

        // Show arrow ONLY when:
        // - Ball is currently held
        // - Ball is not already locked into hole
        // - Hole not yet filled
        bool shouldShow =
            ball.IsHeld &&
            !ball.IsLockedInHole;

        arrowVisual.SetActive(shouldShow);

        if (arrowVisual.activeSelf)
        {
            float y = Mathf.Sin(Time.time * bobSpeed) * bobHeight;
            arrowVisual.transform.localPosition = startLocalPos + Vector3.up * y;
        }
    }

    
}