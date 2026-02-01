using UnityEngine;

public class ReviveBubble : MonoBehaviour
{
    [Header("Float Behavior")]
    [SerializeField] float followSmooth = 8f;
    [SerializeField] Vector3 offsetFromAlivePlayer = new Vector3(0f, 2.5f, 0f);

    [Header("Pop Rules")]
    [SerializeField] bool popOnTouch = true;

    PlayerBubbleState bubbledPlayer;
    Transform followTarget; // usually the other player's transform
    CoopLifeManager lifeManager;

    public void Init(CoopLifeManager manager, PlayerBubbleState playerToRevive, Transform followAlivePlayer)
    {
        lifeManager = manager;
        bubbledPlayer = playerToRevive;
        followTarget = followAlivePlayer;
        transform.position = followTarget.position + offsetFromAlivePlayer;
    }

    void Update()
    {
        if (followTarget == null) return;

        Vector3 targetPos = followTarget.position + offsetFromAlivePlayer;
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * followSmooth);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!popOnTouch) return;
        if (lifeManager == null || bubbledPlayer == null) return;

        // Pop only if the OTHER player touches it (not the bubbled one)
        var otherBubbleState = other.GetComponentInParent<PlayerBubbleState>();
        if (otherBubbleState == null) return;

        if (otherBubbleState.PlayerIndex != bubbledPlayer.PlayerIndex)
        {
            lifeManager.PopBubble(bubbledPlayer.PlayerIndex);
        }
    }
}
