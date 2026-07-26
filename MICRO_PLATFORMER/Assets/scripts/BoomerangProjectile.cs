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
    [SerializeField] Transform hitPoint;
    [SerializeField] float hitRadius = 1f;
    bool returning;

    [Header("Collectibles")]
    [SerializeField] float orbitRadius = 1f;
    [SerializeField] float orbitSpeed = 180f; // degrees per second
    [SerializeField] float orbitHeight = 0.15f;
    List<Transform> collectedItems = new();
    float orbitAngle;

    [SerializeField] GameObject afterImagePrefab;
    [SerializeField] float afterImageInterval = 0.05f;

    float nextAfterImageTime;

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

                foreach (Transform item in collectedItems)
                {
                    if (item == null)
                        continue;

                    item.SetParent(null);
                }

                collectedItems.Clear();

                Destroy(gameObject);
            }
        }

        if (Time.time >= nextAfterImageTime)
        {
            nextAfterImageTime = Time.time + afterImageInterval;

            Instantiate(
                afterImagePrefab,
                transform.position,
                transform.rotation
            );
        }

        transform.Rotate(0f, 1080f * Time.deltaTime, 0f);
        CheckHits();
        UpdateCollectedItems();
    }

    void CheckHits()
    {
        Collider[] hits = Physics.OverlapSphere(hitPoint.position, hitRadius);

        foreach (Collider hit in hits)
        {
           

            // ---------- Enemy ----------
            Enemy enemy = hit.GetComponentInParent<Enemy>();

            if (enemy != null)
            {
                if (hitTargets.Contains(enemy.gameObject))
                    continue;

                hitTargets.Add(enemy.gameObject);

                Vector3 direction = enemy.transform.position - owner.transform.position;

                direction.y = 0f;
                direction.Normalize();

                enemy.TakeKick(direction);

                continue;
            }

            // ---------- Breakable Box ----------
            BreakableBox box = hit.GetComponentInParent<BreakableBox>();

            if (box != null)
            {
                if (hitTargets.Contains(box.gameObject))
                    continue;

                hitTargets.Add(box.gameObject);

                box.Break();

                continue;
            }

            // ---------- Player ----------
            PlayerController3D player = hit.GetComponentInParent<PlayerController3D>();

            if (player != null &&
                player.gameObject != owner.gameObject)
            {
                if (hitTargets.Contains(player.gameObject))
                    continue;

                hitTargets.Add(player.gameObject);

                player.ApplyKickKnockback(owner.transform.position);

                continue;
            }

            CoinPickup coin = hit.GetComponent<CoinPickup>();

            if (coin != null)
            {
                if (hitTargets.Contains(coin.gameObject))
                    continue;

                hitTargets.Add(coin.gameObject);

                collectedItems.Add(coin.transform);

                coin.transform.SetParent(transform);

                continue;
            }

            if (hit.CompareTag("Heart"))
            {
                if (hitTargets.Contains(hit.gameObject))
                    continue;

                hitTargets.Add(hit.gameObject);

                collectedItems.Add(hit.transform);

                hit.transform.SetParent(transform);

                continue;
            }
        }
    }

    void UpdateCollectedItems()
    {
        if (collectedItems.Count == 0)
            return;

        orbitAngle += orbitSpeed * Time.deltaTime;

        float angleStep = 360f / collectedItems.Count;

        for (int i = 0; i < collectedItems.Count; i++)
        {
            if (collectedItems[i] == null)
                continue;

            float angle = orbitAngle + angleStep * i;

            Vector3 offset =
                Quaternion.Euler(0f, angle, 0f) *
                (Vector3.forward * orbitRadius);

            offset += Vector3.up * orbitHeight;

            collectedItems[i].position = transform.position + offset;
        }
    }
}