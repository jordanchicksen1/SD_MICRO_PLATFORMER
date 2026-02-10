using UnityEngine;

public class GemPickupPersistent : MonoBehaviour
{
    [Header("ID")]
    [SerializeField] string levelId = "Level1";
    [SerializeField] string gemId = "Gem_01";

    [Header("Pickup")]
    [SerializeField] int amount = 1;
    [SerializeField] Collider pickupCollider;
    [SerializeField] GameObject visualsRoot;      // set to the mesh/particles parent (optional)
    [SerializeField] AudioSource sfxSource;       // optional (AudioSource on this prefab)

    bool pickedUp;

    void Awake()
    {
        if (!pickupCollider) pickupCollider = GetComponent<Collider>();
        if (visualsRoot == null) visualsRoot = gameObject; // safe default: hide whole object
    }

    void Start()
    {
        // If already collected previously, remove the gem immediately
        if (PersistentGemProgress.Instance != null &&
            PersistentGemProgress.Instance.IsCollected(levelId, gemId))
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (pickedUp) return;

        // Only players
        if (!other.GetComponentInParent<PlayerController3D>())
            return;

        pickedUp = true;

        // Stop double-trigger instantly
        if (pickupCollider) pickupCollider.enabled = false;

        // Hide visuals immediately (feels responsive, prevents re-collect look)
        if (visualsRoot) visualsRoot.SetActive(false);

        // Mark pending for THIS run (so results screen can commit)
        RunGemProgress.Instance?.MarkPending(levelId, gemId);

        // Count for THIS RUN ONLY (banking happens at results screen)
        RunCurrency.Instance?.AddGem(amount);

        // Optional: play SFX safely even if we destroy this object
        if (sfxSource && sfxSource.clip)
            AudioSource.PlayClipAtPoint(sfxSource.clip, transform.position, sfxSource.volume);

        // Destroy the pickup object
        Destroy(gameObject);

        Debug.Log($"[GemPickupPersistent] Collected {levelId}/{gemId}. RunGems now = {RunCurrency.Instance?.LevelGems}");
    }
}
