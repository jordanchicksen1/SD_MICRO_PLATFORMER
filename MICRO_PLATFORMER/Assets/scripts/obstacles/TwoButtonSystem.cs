using UnityEngine;
using System.Collections;

public class ToggleButton : MonoBehaviour
{
    [SerializeField] CoopDoorController controller;

    [Header("Visual")]
    [SerializeField] Transform buttonTop;  // assign the moving button mesh
    [SerializeField] float pressDepth = 0.2f;

    [Header("Trigger rules")]
    [SerializeField] float cooldown = 0.15f; // stops multi-triggers in one landing

    public bool IsPressed { get; private set; }

    Vector3 topStartLocalPos;
    bool canTrigger = true;

    void Awake()
    {
        if (buttonTop == null)
            Debug.LogError("ToggleButton: buttonTop not assigned!", this);

        topStartLocalPos = buttonTop.localPosition;

        // Ensure visual matches state on start
        ApplyVisual();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!canTrigger) return;

        // Must be player
        PlayerController3D player = other.GetComponent<PlayerController3D>();
        if (!player) return;

        // Must be landing / falling down (prevents toggling when brushing sideways)
        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb != null && rb.linearVelocity.y > 0.1f) return;

        Toggle();
        StartCoroutine(Cooldown());
    }

    void Toggle()
    {
        IsPressed = !IsPressed;
        ApplyVisual();

        if (controller != null)
            controller.RecomputeDoor();
    }

    void ApplyVisual()
    {
        if (!buttonTop) return;

        buttonTop.localPosition = IsPressed
            ? topStartLocalPos + Vector3.down * pressDepth
            : topStartLocalPos;
    }

    IEnumerator Cooldown()
    {
        canTrigger = false;
        yield return new WaitForSeconds(cooldown);
        canTrigger = true;
    }
}
