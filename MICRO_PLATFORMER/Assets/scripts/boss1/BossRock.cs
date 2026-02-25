using UnityEngine;

public class BossRock : MonoBehaviour
{
    void Start()
    {
        Destroy(gameObject, 8f);
    }

    void OnCollisionEnter(Collision collision)
    {
        PlayerController3D player = collision.collider.GetComponent<PlayerController3D>();
        if (player)
        {
            player.GetComponent<PlayerHealth>().TakeDamage(1, transform.position);
        }
    }
}