using System.Collections;
using UnityEngine;

public class GemPickupPersistent : MonoBehaviour
{
    [Header("ID")]
    [SerializeField] string levelId = "Level1";
    [SerializeField] string gemId = "Gem_01";

    [Header("Pickup")]
    [SerializeField] int amount = 1;
    [SerializeField] Collider pickupCollider;

    [Header("Visuals")]
    [SerializeField] GameObject realVisualsRoot;        // ONLY visuals
    [SerializeField] GameObject collectedVisualsRoot;   // fake gem visuals (starts OFF)

    [Header("Collected Placeholder Delay")]
    [SerializeField] float showCollectedAfterSeconds = 10f;

    [SerializeField] AudioSource sfxSource; // optional

    bool pickedUp;
    Coroutine showFakeRoutine;

    void Awake()
    {
        if (!pickupCollider) pickupCollider = GetComponent<Collider>();

        // IMPORTANT: these should be CHILD objects, not this GameObject
        // Never default realVisualsRoot to gameObject, because that can disable the script too.

        if (collectedVisualsRoot) collectedVisualsRoot.SetActive(false);
    }

    void Start()
    {
        bool alreadyCollected =
            PersistentGemProgress.Instance != null &&
            PersistentGemProgress.Instance.IsCollected(levelId, gemId);

        if (alreadyCollected)
        {
            // Disable pickup only (keep THIS gameobject active so coroutines can run)
            if (pickupCollider) pickupCollider.enabled = false;

            // Hide real visuals immediately
            if (realVisualsRoot) realVisualsRoot.SetActive(false);

            // Show fake later (only if this component is active)
            if (collectedVisualsRoot && isActiveAndEnabled)
                showFakeRoutine = StartCoroutine(ShowCollectedLater());
        }
        else
        {
            if (realVisualsRoot) realVisualsRoot.SetActive(true);
            if (collectedVisualsRoot) collectedVisualsRoot.SetActive(false);
        }
    }

    IEnumerator ShowCollectedLater()
    {
        yield return new WaitForSeconds(showCollectedAfterSeconds);

        // Safety if scene changed / object destroyed
        if (!this || !isActiveAndEnabled) yield break;

        if (collectedVisualsRoot)
            collectedVisualsRoot.SetActive(true);
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Gem LevelId = " + levelId);

        if (pickedUp) return;
        if (!other.GetComponentInParent<PlayerController3D>()) return;

        pickedUp = true;

        // Cancel delayed fake if it was waiting
        if (showFakeRoutine != null)
        {
            StopCoroutine(showFakeRoutine);
            showFakeRoutine = null;
        }

        if (pickupCollider) pickupCollider.enabled = false;
        if (realVisualsRoot) realVisualsRoot.SetActive(false);
        if (collectedVisualsRoot) collectedVisualsRoot.SetActive(false);

        RunGemProgress.Instance?.MarkPending(levelId, gemId);
        RunCurrency.Instance?.AddGem(amount);

        if (sfxSource && sfxSource.clip)
            AudioSource.PlayClipAtPoint(sfxSource.clip, transform.position, sfxSource.volume);

        Destroy(gameObject);
    }
}
