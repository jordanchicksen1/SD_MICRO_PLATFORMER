using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class BreakableBox : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameObject intactModel;
    [SerializeField] GameObject piecesRoot;
    [SerializeField] GameObject coinPrefab;
    [SerializeField] AudioSource boxBreakSFX;

    [Header("VFX")]
    [SerializeField] VisualEffect smokePoof;     // prefab with VisualEffect
    [SerializeField] float vfxLifetime = 2f;     // how long before we destroy the VFX object

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
            Debug.LogError("PiecesRoot is NULL on BreakableBox.", this);
            return;
        }

        piecesRoot.SetActive(true);
        if (coinPrefab != null)
            Instantiate(coinPrefab, transform.position, Quaternion.identity);

        // Detach pieces so destroying the box doesn't kill them
        piecesRoot.transform.SetParent(null, true);

        foreach (Transform piece in piecesRoot.transform)
        {
            var rb = piece.GetComponent<Rigidbody>();
            if (rb == null) continue;

            rb.isKinematic = false;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.AddExplosionForce(explodeForce, transform.position, explodeRadius, upForce, ForceMode.Impulse);
        }

        // Hide/disable original collider & visuals
        var c = GetComponent<Collider>();
        if (c != null) c.enabled = false;
        foreach (var r in GetComponentsInChildren<Renderer>()) r.enabled = false;

        // Destroy the original box root soon
        Destroy(gameObject, 0.35f);

        // Handle the "pieces -> vfx -> cleanup" sequence
        StartCoroutine(PiecesThenVFXThenCleanup());
    }

    IEnumerator PiecesThenVFXThenCleanup()
    {
        // wait until you're ready to remove pieces
        yield return new WaitForSeconds(destroyAfter);

        // choose where to play the vfx (center of pieces)
        Vector3 pos = GetCenterOfPieces(piecesRoot);

        // spawn and play vfx
        if (smokePoof != null)
        {
            VisualEffect vfx = Instantiate(smokePoof, pos, Quaternion.identity);
            vfx.Reinit();
            vfx.Play();
            Destroy(vfx.gameObject, vfxLifetime);
        }

        // destroy pieces after spawning vfx
        if (piecesRoot != null)
            Destroy(piecesRoot);
    }

    Vector3 GetCenterOfPieces(GameObject root)
    {
        if (root == null) return transform.position;

        var renderers = root.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0) return root.transform.position;

        Bounds b = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++)
            b.Encapsulate(renderers[i].bounds);

        return b.center;
    }
}
