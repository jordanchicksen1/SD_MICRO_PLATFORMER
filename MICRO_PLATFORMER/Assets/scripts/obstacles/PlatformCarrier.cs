using UnityEngine;

public class PlatformCarrier : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        TryParent(collision);
    }

    void OnCollisionStay(Collision collision)
    {
        TryParent(collision);
    }

    void OnCollisionExit(Collision collision)
    {
        PlayerController3D player = collision.collider.GetComponent<PlayerController3D>();
        if (player)
            player.transform.SetParent(null, true);
    }

    void TryParent(Collision collision)
    {
        PlayerController3D player = collision.collider.GetComponent<PlayerController3D>();
        if (!player) return;

        // Only parent if the player is on top of the platform
        // (contact normal points upward from the platform)
        if (collision.contactCount > 0 && collision.contacts[0].normal.y > 0.5f)
        {
            player.transform.SetParent(transform, true);
        }
    }
}
