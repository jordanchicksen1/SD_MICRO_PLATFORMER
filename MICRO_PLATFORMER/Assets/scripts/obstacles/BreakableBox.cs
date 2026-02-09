using UnityEngine;

public class BreakableBox : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameObject intactModel;
    [SerializeField] GameObject piecesRoot;
    [SerializeField] GameObject coinPrefab;
    [SerializeField] AudioSource boxBreakSFX;

    [Header("Break Settings")]
    [SerializeField] float explodeForce = 4f;
    [SerializeField] float explodeRadius = 1.5f;
    [SerializeField] float upForce = 1.5f;
    [SerializeField] float destroyAfter = 3f;

    bool broken;

    void Awake()
    {
        if (piecesRoot != null)
            piecesRoot.SetActive(false);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (broken) return;

        PlayerController3D player = collision.collider.GetComponentInParent<PlayerController3D>();
        if (player == null) player = collision.collider.GetComponentInParent<PlayerController3D>();
        if (player == null) return;
        
        if (player.IsGroundPounding())
            Break();
    }

    void Break()
    {
        if (broken) return;
        broken = true;

        // Hide intact
        if (intactModel != null)
            intactModel.SetActive(false);
            boxBreakSFX.Play();

        if (piecesRoot == null)
        {
            Debug.LogError("PiecesRoot is NULL on BreakableBox. Assign it in the inspector.", this);
            return;
        }

        // Turn on pieces
        piecesRoot.SetActive(true);
        Instantiate(coinPrefab, transform.position, Quaternion.identity);
        
        // IMPORTANT: detach piecesRoot from THIS object BEFORE we disable/destroy anything
        piecesRoot.transform.SetParent(null, true);

        // Enable physics on all pieces
        foreach (Transform piece in piecesRoot.transform)
        {
            Rigidbody rb = piece.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.AddExplosionForce(explodeForce, transform.position, explodeRadius, upForce, ForceMode.Impulse);
            }
            else
            {
                Debug.LogWarning($"Piece '{piece.name}' has no Rigidbody.", piece);
            }
        }

        // Turn off collisions/visuals on the original box (don’t destroy immediately)
        Collider c = GetComponent<Collider>();
        if (c != null) c.enabled = false;

        // Optional: hide the root object so you don’t see anything weird
        foreach (var r in GetComponentsInChildren<Renderer>())
            r.enabled = false;

        Destroy(gameObject, 0.35f);                 // destroy the old box root
        Destroy(piecesRoot, destroyAfter);         // clean up pieces later (optional)
    }
}
