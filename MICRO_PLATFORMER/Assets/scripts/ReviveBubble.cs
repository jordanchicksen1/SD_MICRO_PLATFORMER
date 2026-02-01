using UnityEngine;

public class ReviveBubble : MonoBehaviour
{
    [Header("Float Behavior")]
    [SerializeField] float followSmooth = 8f;
    [SerializeField] Vector3 offsetFromAlivePlayer = new Vector3(0f, 3.0f, 0f);
    float bubbleBaseY;

    [Header("Pop Rules")]
    [SerializeField] bool popOnTouch = true;
    [SerializeField] float popGraceTime = 0.35f; // NEW: prevents instant pop on spawn

    float spawnTime;

    PlayerBubbleState bubbledPlayer;
    Transform followTarget;
    CoopLifeManager lifeManager;

    public void Init(CoopLifeManager manager, PlayerBubbleState playerToRevive, Transform followAlivePlayer)
    {
        lifeManager = manager;
        bubbledPlayer = playerToRevive;
        followTarget = followAlivePlayer;

        spawnTime = Time.time;

        transform.position = followTarget.position + offsetFromAlivePlayer;

        bubbleBaseY = transform.position.y; // lock hover height at spawn
    }

    void Update()
    {
        if (followTarget == null) return;

        // Follow target only in XZ so jumping doesn't drag the bubble up
        Vector3 target = followTarget.position + offsetFromAlivePlayer;

        Vector3 current = transform.position;
        Vector3 desired = new Vector3(target.x, current.y, target.z);

        transform.position = Vector3.Lerp(transform.position, desired, Time.deltaTime * followSmooth);

        // nice bubble bob (optional)
        float bob = Mathf.Sin(Time.time * 2.5f) * 0.25f;
        transform.position = new Vector3(transform.position.x, bubbleBaseY + bob, transform.position.z);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!popOnTouch) return;
        if (Time.time - spawnTime < popGraceTime) return; // NEW: ignore touches at spawn

        if (lifeManager == null || bubbledPlayer == null) return;

        var otherBubbleState = other.GetComponentInParent<PlayerBubbleState>();
        if (otherBubbleState == null) return;

        if (otherBubbleState.PlayerIndex != bubbledPlayer.PlayerIndex)
        {
            lifeManager.PopBubble(bubbledPlayer.PlayerIndex);
        }
    }
}
