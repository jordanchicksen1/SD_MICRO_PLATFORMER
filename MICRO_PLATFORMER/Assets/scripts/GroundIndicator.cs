using UnityEngine;

public class GroundIndicator : MonoBehaviour
{
    [Header("Ground Indicator")]
    [SerializeField] float groundOffset = 0.02f;
    [SerializeField] LayerMask groundLayer;

    [Header("Other Player Arrow")]
    [SerializeField] Transform otherPlayerArrow;
    [SerializeField] Renderer otherPlayerArrowRenderer;

    [SerializeField] float arrowDistanceFromCenter = 0.6f;
    [SerializeField] float arrowHeight = 0.02f;

    [SerializeField] float arrowFadeStartDistance = 15f;
    [SerializeField] float arrowFullAlphaDistance = 30f;

    Transform target;
    Transform otherPlayer;

    Material arrowMaterial;

    public void SetTarget(Transform player)
    {
        target = player;
    }

    public void SetOtherPlayer(Transform player)
    {
        otherPlayer = player;
    }

    public void SetArrowMaterial(Material material)
    {
        if (otherPlayerArrowRenderer == null)
            return;

        arrowMaterial = material;
        otherPlayerArrowRenderer.material = arrowMaterial;
    }

    void LateUpdate()
    {
        if (!target)
            return;

        UpdateGroundPosition();
        UpdateOtherPlayerArrow();
    }

    void UpdateGroundPosition()
    {
        Vector3 rayOrigin = target.position + Vector3.up;

        if (Physics.Raycast(
            rayOrigin,
            Vector3.down,
            out RaycastHit hit,
            30f,
            groundLayer))
        {
            transform.position =
                hit.point + Vector3.up * groundOffset;
        }
        else
        {
            transform.position = new Vector3(
                target.position.x,
                target.position.y - 1f,
                target.position.z
            );
        }
    }

    void UpdateOtherPlayerArrow()
    {
        if (otherPlayerArrow == null)
            return;

        if (otherPlayer == null)
        {
            otherPlayerArrow.gameObject.SetActive(false);
            return;
        }

        Vector3 direction =
            otherPlayer.position - target.position;

        direction.y = 0f;

        float distance = direction.magnitude;

        // Only show the arrow during split-screen.
        CoopCameraController cameraController =
            FindFirstObjectByType<CoopCameraController>();

        if (cameraController == null ||
            !cameraController.IsSplitScreen())
        {
            otherPlayerArrow.gameObject.SetActive(false);
            return;
        }

        // Fade from invisible to fully visible
        // as the players move farther apart.
        float alpha = Mathf.InverseLerp(
            arrowFadeStartDistance,
            arrowFullAlphaDistance,
            distance
        );

        if (alpha <= 0f)
        {
            otherPlayerArrow.gameObject.SetActive(false);
            return;
        }

        otherPlayerArrow.gameObject.SetActive(true);

        if (direction.sqrMagnitude > 0.001f)
        {
            direction.Normalize();

            // Position the arrow around the edge
            // of the ground circle.
            otherPlayerArrow.position =
                transform.position +
                direction * arrowDistanceFromCenter +
                Vector3.up * arrowHeight;

            // Point the arrow toward the other player.
            otherPlayerArrow.rotation =
                Quaternion.LookRotation(direction);
        }

        SetArrowAlpha(alpha);
    }

    void SetArrowAlpha(float alpha)
    {
        if (arrowMaterial == null)
            return;

        if (arrowMaterial.HasProperty("_BaseColor"))
        {
            Color color =
                arrowMaterial.GetColor("_BaseColor");

            color.a = alpha;

            arrowMaterial.SetColor(
                "_BaseColor",
                color
            );
        }
        else if (arrowMaterial.HasProperty("_Color"))
        {
            Color color =
                arrowMaterial.GetColor("_Color");

            color.a = alpha;

            arrowMaterial.SetColor(
                "_Color",
                color
            );
        }
    }
}