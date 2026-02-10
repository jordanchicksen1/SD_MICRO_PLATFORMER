using UnityEngine;

public class GemPickup : MonoBehaviour
{
    [SerializeField] string levelId = "Level1";
    [SerializeField] string gemId = "Gem_01";
    [SerializeField] int amount = 1;

    void Start()
    {
        // If already permanently collected (build) -> remove it
        if (PersistentGemProgress.Instance != null &&
            PersistentGemProgress.Instance.IsCollected(levelId, gemId))
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.GetComponentInParent<PlayerController3D>()) return;

        RunCurrency.Instance?.AddGem(amount);
        RunGemProgress.Instance?.MarkPending(levelId, gemId);

        Destroy(gameObject);

        Debug.Log($"[GemPickup] Collected {levelId}/{gemId}. RunGems now = {RunCurrency.Instance?.LevelGems}");

    }
}
