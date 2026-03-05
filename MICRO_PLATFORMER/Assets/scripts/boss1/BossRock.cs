using UnityEngine;
using System.Collections;

public class BossRock : MonoBehaviour
{
    bool falling = true;
    [SerializeField] GameObject shadowPrefab;
    [SerializeField] LayerMask groundLayer;

    GameObject shadow;

    void Start()
    {
        shadow = Instantiate(shadowPrefab);

        GroundIndicator indicator = shadow.GetComponent<GroundIndicator>();

        if (indicator != null)
        {
            indicator.SetTarget(transform);
        }

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
        // If the rock hits the ground
        if (((1 << collision.gameObject.layer) & groundLayer) != 0)
        {
            falling = false;

            if (shadow != null)
            {
                Destroy(shadow); // remove shadow once rock lands
            }

            return; // DO NOT destroy the rock
        }

        // If it hits a player while falling
        if (falling)
        {
            PlayerController3D player = collision.collider.GetComponent<PlayerController3D>();

            if (player)
            {
                player.GetComponent<PlayerHealth>().TakeDamage(1, transform.position);

                if (shadow != null)
                    Destroy(shadow);

                Destroy(gameObject); // rock breaks on player
            }
        }
    }
}