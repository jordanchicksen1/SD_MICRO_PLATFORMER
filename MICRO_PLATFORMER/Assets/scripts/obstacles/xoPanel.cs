using UnityEngine;

public class QuestionPanel : MonoBehaviour
{
    [Header("Visuals")]
    [SerializeField] GameObject questionVisual;
    [SerializeField] GameObject exclaimVisual;

    [Header("Stomp Detection")]
    [SerializeField] float stompTolerance = 0.05f;
    [SerializeField] float toggleCooldown = 0.2f;
    float nextToggleTime;


    [Header("References")]
    [SerializeField] QuestionPanelManager manager;

    bool isActivated;

    Collider panelCol;

    void Awake()
    {
        panelCol = GetComponent<Collider>();
        SetVisual(false);
    }

    void SetVisual(bool activated)
    {
        isActivated = activated;

        if (questionVisual) questionVisual.SetActive(!activated);
        if (exclaimVisual) exclaimVisual.SetActive(activated);
    }

    void OnCollisionEnter(Collision collision)
    {
        // Only players can toggle
        PlayerController3D player = collision.collider.GetComponent<PlayerController3D>();
        if (!player) return;

        // Must land from above
        Collider playerCol = player.GetComponent<Collider>();
        if (!playerCol || panelCol == null) return;

        float playerFeetY = playerCol.bounds.min.y;
        float panelTopY = panelCol.bounds.max.y;

        bool fromAbove = playerFeetY >= panelTopY - stompTolerance;

        Rigidbody prb = player.GetComponent<Rigidbody>();
        bool fallingOrLanding = prb != null && prb.linearVelocity.y <= 1f;

        if (!fromAbove || !fallingOrLanding)
            return;

        Toggle();
    }

    void Toggle()
    {
        if (Time.time < nextToggleTime) return;
        nextToggleTime = Time.time + toggleCooldown;


        // If it was off -> turn on and add count
        if (!isActivated)
        {
            SetVisual(true);
            if (manager != null) manager.AddPanel();
        }
        // If it was on -> turn off and remove count
        else
        {
            SetVisual(false);
            if (manager != null) manager.RemovePanel();
        }
    }
}
