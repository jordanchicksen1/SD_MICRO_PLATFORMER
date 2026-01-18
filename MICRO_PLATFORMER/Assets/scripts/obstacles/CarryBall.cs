using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CarryBall : MonoBehaviour, IInteractable
{
    [Header("Carry Debuff")]
    [SerializeField] float carryMoveMultiplier = 0.8f;
    [SerializeField] float carryJumpMultiplier = 0.85f;

    Rigidbody rb;
    Collider col;

    PlayerController3D holder;

    bool lockedInHole; // once in hole, can't be picked up

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }

    public bool IsHeld => holder != null;
    public bool IsLockedInHole => lockedInHole;

    public void Interact(PlayerController3D player)
    {
        // Toggle: if not held -> pick up, if held by this player -> drop
        if (lockedInHole) return;

        if (holder == null)
            PickUp(player);
        else if (holder == player)
            Drop();
    }

    public void PickUp(PlayerController3D player)
    {
        if (lockedInHole) return;
        if (player == null) return;

        if (player.HoldPoint == null)
        {
            Debug.LogError("HoldPoint is NULL on player! Assign it in the Player prefab.", player);
            return;
        }

        holder = player;

        PlayerAnimator anim = holder.GetComponentInChildren<PlayerAnimator>();
        if (anim != null) anim.SetCarrying(true);


        // Apply debuff
        holder.SetCarryModifiers(carryMoveMultiplier, carryJumpMultiplier);

        // Disable physics so it doesn't fight you
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;

        // Optional: stop weird collisions with player while held
        if (col != null) col.enabled = false;

        // Parent to HoldPoint and snap
        transform.SetParent(player.HoldPoint, worldPositionStays: false);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        Debug.Log("Ball picked up!");
    }

    public void Drop()
    {
        if (holder == null) return;

        // Remove debuff FIRST
        holder.SetCarryModifiers(1f, 1f);

        // Unparent
        transform.SetParent(null);

        // Re-enable physics
        rb.isKinematic = false;

        if (col != null) col.enabled = true;

        PlayerAnimator anim = holder.GetComponentInChildren<PlayerAnimator>();
        if (anim != null) anim.SetCarrying(false);


        holder = null;

        Debug.Log("Ball dropped!");
    }

    // Called by the hole when the ball is placed successfully
    public void LockIntoHole(Transform snapPoint)
    {
        // If it was held, restore player stats
        if (holder != null)
        {
            holder.SetCarryModifiers(1f, 1f);
            PlayerAnimator anim = holder.GetComponentInChildren<PlayerAnimator>();
            if (anim != null) anim.SetCarrying(false);

            holder = null;
        }

        lockedInHole = true;

        transform.SetParent(null);
        transform.position = snapPoint.position;
        transform.rotation = snapPoint.rotation;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;

        if (col != null) col.enabled = false; // no more collisions
    }
}
