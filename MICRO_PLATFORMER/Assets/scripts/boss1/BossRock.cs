using UnityEngine;
using System.Collections;

public class BossRock : MonoBehaviour
{
    bool falling = true;

    void Start()
    {
        Destroy(gameObject, 10f); // stays on battlefield
        StartCoroutine(StopFallingAfterDelay());
    }

    IEnumerator StopFallingAfterDelay()
    {
        yield return new WaitForSeconds(1f); // time it takes to land
        falling = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!falling) return;

        PlayerController3D player = collision.collider.GetComponent<PlayerController3D>();
        if (player)
        {
            player.GetComponent<PlayerHealth>().TakeDamage(1, transform.position);
        }
    }
}