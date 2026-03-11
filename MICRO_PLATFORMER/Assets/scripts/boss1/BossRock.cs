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
        //StartCoroutine(StopFallingAfterDelay());
    }

    IEnumerator StopFallingAfterDelay()
    {
        yield return new WaitForSeconds(1f); // time it takes to land
        falling = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        PlayerController3D player = collision.collider.GetComponent<PlayerController3D>();

        // ONLY damage if the rock is still falling
        if (falling && player)
        {
            player.GetComponent<PlayerHealth>().TakeDamage(1, transform.position);
        }

        // If we hit the ground, stop falling
        if (falling && ((1 << collision.gameObject.layer) & groundLayer) != 0)
        {
            falling = false;

            // destroy the shadow when the rock lands
            if (shadow != null)
                Destroy(shadow);
        }
    }
}