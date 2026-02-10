using UnityEngine;

public class PersistentGemPickup : MonoBehaviour
{
    [Header("ID")]
    [SerializeField] string levelId = "Level1";
    [SerializeField] string gemId = "Gem_01";

    [Header("Pickup")]
    [SerializeField] int amount = 1;
    [SerializeField] Collider pickupCollider;
    [SerializeField] GameObject visualsRoot; // mesh/particles to hide immediately
   // [SerializeField] AudioSource sfx;        // optional

    bool pickedUp;

    void Awake()
    {
        if (!pickupCollider) pickupCollider = GetComponent<Collider>();
    }

    void Start()
    {
        // If already collected in a previous run, remove it immediately
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

        // Already collected? (safety)
        if (PersistentGemProgress.Instance != null &&
            PersistentGemProgress.Instance.IsCollected(levelId, gemId))
        {
            pickedUp = true;
            if (pickupCollider) pickupCollider.enabled = false;
            Destroy(gameObject);
            return;
        }

        pickedUp = true;

        // Stop any double-trigger instantly
        if (pickupCollider) pickupCollider.enabled = false;

        // Hide visuals immediately so you can't "collect again" visually
        if (visualsRoot) visualsRoot.SetActive(false);

        // Mark progress first
        PersistentGemProgress.Instance?.TryMarkCollected(levelId, gemId);

        // Add currency
        RunCurrency.Instance?.AddGem(amount);
        CurrencyManager.Instance?.AddGems(amount);

        // Play SFX safely even after object is destroyed:
      //  if (sfx && sfx.clip)
            //AudioSource.PlayClipAtPoint(sfx.clip, transform.position, sfx.volume);

        Destroy(gameObject);
    }
}
