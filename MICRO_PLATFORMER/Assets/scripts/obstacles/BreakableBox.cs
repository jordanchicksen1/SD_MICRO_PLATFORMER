using System.Collections;
using UnityEngine;

public class BreakableBox : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameObject intactModel;
    [SerializeField] GameObject piecesRoot;
    [SerializeField] GameObject coinPrefab;
    [SerializeField] AudioSource boxBreakSFX;

    [Header("VFX (Prefab)")]
    [SerializeField] GameObject breakEndVFXPrefab;
    [SerializeField] float breakEndVFXLifetime = 2f;

    [Header("Break Settings")]
    [SerializeField] float explodeForce = 4f;
    [SerializeField] float explodeRadius = 1.5f;
    [SerializeField] float upForce = 1.5f;
    [SerializeField] float piecesDestroyAfter = 3f;

    bool broken;

    void Awake()
    {
        if (piecesRoot != null)
            piecesRoot.SetActive(false);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (broken) return;

        var player = collision.collider.GetComponentInParent<PlayerController3D>();
        if (player == null) return;

        if (player.IsGroundPounding())
            Break();
    }

    void Break()
    {
        if (broken) return;
        broken = true;

        if (intactModel != null) intactModel.SetActive(false);
        if (boxBreakSFX != null) boxBreakSFX.Play();

        if (piecesRoot == null)
        {
            Debug.LogError("PiecesRoot is NULL on BreakableBox. Assign it in the inspector.", this);
            return;
        }

        // Enable + detach pieces so they can live independently
        piecesRoot.SetActive(true);
        piecesRoot.transform.SetParent(null, true);

        if (coinPrefab != null)
            Instantiate(coinPrefab, transform.position, Quaternion.identity);

        foreach (Transform piece in piecesRoot.transform)
        {
            Rigidbody rb = piece.GetComponent<Rigidbody>();
            if (rb == null) continue;

            rb.isKinematic = false;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.AddExplosionForce(explodeForce, transform.position, explodeRadius, upForce, ForceMode.Impulse);
        }

        // Make the box "gone" immediately, but DON'T destroy this object yet
        Collider c = GetComponent<Collider>();
        if (c != null) c.enabled = false;

        foreach (var r in GetComponentsInChildren<Renderer>())
            r.enabled = false;

        StartCoroutine(PiecesDieThenVFXThenCleanup());
    }

    IEnumerator PiecesDieThenVFXThenCleanup()
    {
        yield return new WaitForSeconds(piecesDestroyAfter);

        Vector3 vfxPos = GetPiecesCenter(piecesRoot);

        if (breakEndVFXPrefab != null)
        {
            GameObject vfx = Instantiate(breakEndVFXPrefab, vfxPos, Quaternion.identity);
            Destroy(vfx, breakEndVFXLifetime);
        }
        else
        {
            Debug.LogWarning("BreakEndVFXPrefab not assigned on BreakableBox.", this);
        }

        if (piecesRoot != null)
            Destroy(piecesRoot);

        // now it's safe to destroy the invisible coroutine-runner object
        Destroy(gameObject);
    }

    Vector3 GetPiecesCenter(GameObject root)
    {
        if (root == null) return transform.position;

        Renderer[] rs = root.GetComponentsInChildren<Renderer>();
        if (rs.Length == 0) return root.transform.position;

        Bounds b = rs[0].bounds;
        for (int i = 1; i < rs.Length; i++)
            b.Encapsulate(rs[i].bounds);

        return b.center;
    }
}
