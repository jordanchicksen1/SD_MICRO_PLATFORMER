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
    List<BoomerangTarget> targets = new();
    int currentTargetIndex = 0;
    bool targetedThrow;

    [Header("Collectibles")]
    [SerializeField] float orbitRadius = 1f;
    [SerializeField] float orbitSpeed = 180f; // degrees per second
    [SerializeField] float orbitHeight = 0.15f;
    List<Transform> collectedItems = new();
    float orbitAngle;

    [SerializeField] GameObject afterImagePrefab;
    [SerializeField] float afterImageInterval = 0.05f;

    float nextAfterImageTime;

    [Header("SFX")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioSource flyingSource;
    [SerializeField] AudioClip throwSFX;
    [SerializeField] AudioClip flightLoopSFX;
    [SerializeField] AudioClip catchSFX;
    [SerializeField] AudioClip collectPingSFX;


    private void Start()
    {
        audioSource.PlayOneShot(throwSFX);

        flyingSource.clip = flightLoopSFX;
        flyingSource.loop = true;
        flyingSource.Play();
    }
    public void Init(PlayerCombat combat, Vector3 throwDirection)
    {
        owner = combat;

        startPosition = transform.position;

        direction = throwDirection.normalized;
    }

    public void Init(PlayerCombat combat, Vector3 throwDirection, List<BoomerangTarget> selectedTargets)
    {
        owner = combat;

        startPosition = transform.position;

        direction = throwDirection.normalized;

        targets = selectedTargets;

        targetedThrow = targets.Count > 0;
    }

    void Update()
    {
        if (!returning)
        {
            if (targetedThrow)
            {
                UpdateTargetedMovement();
            }
            else
            {
                transform.position += direction * speed * Time.deltaTime;

                if (Vector3.Distance(startPosition, transform.position) >= maxDistance)
                {
                    returning = true;
                }
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
                //audioSource.Stop();
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

    void UpdateTargetedMovement()
    {
        if (targets.Count == 0)
        {
            returning = true;
            return;
        }

        if (currentTargetIndex >= targets.Count)
        {
            returning = true;
            return;
        }

        BoomerangTarget currentTarget = targets[currentTargetIndex];

        if (currentTarget == null)
        {
            currentTargetIndex++;
            return;
        }

        Vector3 targetDirection = (currentTarget.transform.position - transform.position).normalized;

        transform.position += targetDirection * speed * Time.deltaTime;

        
    }

    void AdvanceToNextTarget()
    {
        if (currentTargetIndex < targets.Count)
        {
            BoomerangTarget completedTarget = targets[currentTargetIndex];

            if (completedTarget != null)
            {
                completedTarget.HideMarker(owner.PlayerIndex);
            }
        }

        currentTargetIndex++;

        if (currentTargetIndex >= targets.Count)
        {
            returning = true;
        }
    }

    void RemoveCollectedTarget(BoomerangTarget collectedTarget)
    {
        if (!targetedThrow || collectedTarget == null)
            return;

        int collectedIndex = targets.IndexOf(collectedTarget);

        if (collectedIndex == -1)
            return;

        // The current target is handled by AdvanceToNextTarget(),
        // which already hides its marker.
        if (collectedIndex == currentTargetIndex)
            return;

        // This was a future target that was collected early.
        if (collectedIndex > currentTargetIndex)
        {
            collectedTarget.HideMarker(owner.PlayerIndex);

            targets.RemoveAt(collectedIndex);
        }
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

                if (targetedThrow &&
                    currentTargetIndex < targets.Count &&
                    targets[currentTargetIndex] == enemy.GetComponent<BoomerangTarget>())
                {
                    AdvanceToNextTarget();
                }

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

                if (targetedThrow &&
                    currentTargetIndex < targets.Count &&
                    targets[currentTargetIndex] == box.GetComponent<BoomerangTarget>())
                {
                    AdvanceToNextTarget();
                }

                continue;
            }

            //---------- Boss Hand -------
            BossHand hand = hit.GetComponentInParent<BossHand>();

            if (hand != null)
            {
                hand.DamageHand();
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

                BoomerangTarget target =
                    player.GetComponent<BoomerangTarget>();

                if (targetedThrow &&
                    currentTargetIndex < targets.Count &&
                    targets[currentTargetIndex] == target)
                {
                    AdvanceToNextTarget();
                }

                continue;
            }

            CoinPickup coin = hit.GetComponentInParent<CoinPickup>();

            if (coin != null)
            {
                if (hitTargets.Contains(coin.gameObject))
                    continue;

                BoomerangTarget target =
                    coin.GetComponentInParent<BoomerangTarget>();

                bool isCurrentTarget =
                    targetedThrow &&
                    currentTargetIndex < targets.Count &&
                    targets[currentTargetIndex] == target;

                // If this coin is a future target, remember that
                // we collected it before reaching it.
                if (targetedThrow && target != null)
                {
                    RemoveCollectedTarget(target);
                }

                hitTargets.Add(coin.gameObject);

                audioSource.PlayOneShot(collectPingSFX);

                if (isCurrentTarget)
                {
                    AdvanceToNextTarget();
                }

                collectedItems.Add(coin.transform);

                coin.transform.SetParent(transform);

                continue;
            }

            if (hit.CompareTag("Heart"))
            {
                if (hitTargets.Contains(hit.gameObject))
                    continue;

                BoomerangTarget target =
                    hit.GetComponentInParent<BoomerangTarget>();

                bool isCurrentTarget =
                    targetedThrow &&
                    currentTargetIndex < targets.Count &&
                    targets[currentTargetIndex] == target;

                // If this heart is a future selected target,
                // remove it from the remaining target route.
                if (targetedThrow && target != null)
                {
                    RemoveCollectedTarget(target);
                }

                hitTargets.Add(hit.gameObject);

                audioSource.PlayOneShot(collectPingSFX);

                // If this is the current target, advance normally.
                if (isCurrentTarget)
                {
                    AdvanceToNextTarget();
                }

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