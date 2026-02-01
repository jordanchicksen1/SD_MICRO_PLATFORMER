using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBubbleState : MonoBehaviour
{
    [Header("Hide/Disable")]
    [SerializeField] GameObject visualsRoot;      // drag your model/visual root here
    [SerializeField] Collider[] collidersToDisable;

    Rigidbody rb;
    PlayerInput playerInput;
    PlayerHealth health;

    public bool IsBubbled { get; private set; }
    public int PlayerIndex { get; private set; }  // 0 or 1
    public Vector3 LastSafePosition { get; private set; }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
        health = GetComponent<PlayerHealth>();
        PlayerIndex = playerInput ? playerInput.playerIndex : 0;

        if (collidersToDisable == null || collidersToDisable.Length == 0)
            collidersToDisable = GetComponentsInChildren<Collider>();
    }

    void FixedUpdate()
    {
        // Keep a “safe-ish” position while alive
        if (!IsBubbled && health != null && health.CurrentHealth > 0)
        {
            // You can replace this with a ground-check if you want:
            LastSafePosition = transform.position;
        }
    }

    public void EnterBubble()
    {
        IsBubbled = true;

        if (playerInput) playerInput.enabled = false;

        if (rb)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        if (visualsRoot) visualsRoot.SetActive(false);

        foreach (var c in collidersToDisable)
            if (c) c.enabled = false;
    }

    public void ExitBubble(Vector3 respawnPos, int reviveHp = 1)
    {
        IsBubbled = false;

        transform.position = respawnPos;

        if (rb) rb.isKinematic = false;
        foreach (var c in collidersToDisable)
            if (c) c.enabled = true;

        if (visualsRoot) visualsRoot.SetActive(true);
        if (playerInput) playerInput.enabled = true;

        if (health) health.ReviveTo(reviveHp);
    }
}
