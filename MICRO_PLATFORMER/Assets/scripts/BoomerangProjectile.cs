using UnityEngine;
using System.Collections.Generic;

public class BoomerangProjectile : MonoBehaviour
{
    PlayerCombat owner;

    Vector3 startPosition;
    Vector3 direction;
    HashSet<GameObject> hitTargets = new HashSet<GameObject>();
    [SerializeField] float speed = 20f;
    [SerializeField] float maxDistance = 10f;

    bool returning;

    public void Init(PlayerCombat combat, Vector3 throwDirection)
    {
        owner = combat;

        startPosition = transform.position;

        direction = throwDirection.normalized;
    }

    void Update()
    {
        if (!returning)
        {
            transform.position += direction * speed * Time.deltaTime;

            if (Vector3.Distance(startPosition, transform.position) >= maxDistance)
            {
                returning = true;
            }
        }
        else
        {
            Vector3 returnDir =
                (owner.transform.position - transform.position).normalized;

            transform.position +=
                returnDir * speed * Time.deltaTime;

            if (Vector3.Distance(transform.position, owner.transform.position) < 1f)
            {
                owner.BoomerangReturned();

                Destroy(gameObject);
            }
        }

        transform.Rotate(0f, 1080f * Time.deltaTime, 0f);
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Boomerang hit: {other.name} ({other.gameObject.layer})");

        // Ignore the player who threw it
        if (other.transform.IsChildOf(owner.transform))
            return;

        GameObject target = other.transform.root.gameObject;

        // Already hit this object
        if (hitTargets.Contains(target))
            return;

        hitTargets.Add(target);

        // ---------- Enemy ----------
        Enemy enemy = other.GetComponentInParent<Enemy>();

        if (enemy != null)
        {
            Vector3 direction = enemy.transform.position - owner.transform.position;

            direction.y = 0f;
            direction.Normalize();

            enemy.TakeKick(direction);
            return;
        }

        // ---------- Breakable Box ----------
        BreakableBox box = other.GetComponentInParent<BreakableBox>();

        if (box != null)
        {
            box.Break();
            return;
        }

        // ---------- Player ----------
        PlayerController3D player = other.GetComponentInParent<PlayerController3D>();

        if (player != null &&
            player.gameObject != owner.gameObject)
        {
            player.ApplyKickKnockback(owner.transform.position);
        }
    }
}