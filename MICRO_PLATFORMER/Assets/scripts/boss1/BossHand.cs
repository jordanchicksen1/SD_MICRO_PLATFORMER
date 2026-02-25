using UnityEngine;
using System.Collections;

public class BossHand : MonoBehaviour
{
    [Header("Attack")]
    [SerializeField] float followSpeed = 3f;
    [SerializeField] float slamHeight = 8f;
    [SerializeField] float slamSpeed = 15f;
    [SerializeField] float telegraphTime = 0.7f;

    [Header("Rock")]
    [SerializeField] GameObject rockPrefab;
    [SerializeField] int rockCount = 3;

    BossController boss;

    Vector3 startPos;
    bool attacking;
    bool stunned;

    public bool IsStunned => stunned;

    public void SetBoss(BossController b)
    {
        boss = b;
    }

    void Start()
    {
        startPos = transform.position;
        StartCoroutine(AttackLoop());
    }

    IEnumerator AttackLoop()
    {
        while (true)
        {
            if (!stunned)
                yield return SlamAttack();

            yield return new WaitForSeconds(1.5f);
        }
    }

    IEnumerator SlamAttack()
    {
        attacking = true;

        Transform target = FindClosestPlayer();

        // Follow above player
        Vector3 hoverPos = target.position + Vector3.up * slamHeight;

        float t = 0f;
        while (t < telegraphTime)
        {
            transform.position = Vector3.Lerp(transform.position, hoverPos, followSpeed * Time.deltaTime);
            t += Time.deltaTime;
            yield return null;
        }

        // Slam down
        while (transform.position.y > target.position.y)
        {
            transform.position += Vector3.down * slamSpeed * Time.deltaTime;
            yield return null;
        }

        CheckImpact();

        // Return to start
        while (Vector3.Distance(transform.position, startPos) > 0.1f)
        {
            transform.position = Vector3.Lerp(transform.position, startPos, 4f * Time.deltaTime);
            yield return null;
        }

        attacking = false;
    }

    void CheckImpact()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, 2f);

        bool hitRock = false;

        foreach (var h in hits)
        {
            if (h.CompareTag("Rock"))
            {
                hitRock = true;
                Destroy(h.gameObject);
            }
        }

        if (hitRock)
        {
            StartCoroutine(Stun());
        }
        else
        {
            SpawnRocks();
        }
    }

    void SpawnRocks()
    {
        for (int i = 0; i < rockCount; i++)
        {
            Instantiate(rockPrefab, transform.position + Random.insideUnitSphere * 2f, Quaternion.identity);
        }
    }

    IEnumerator Stun()
    {
        stunned = true;

        // TODO: play stun animation
        boss.OnHandStunned();

        yield return new WaitForSeconds(4f);

        stunned = false;
    }

    public void ResetHand()
    {
        stunned = false;
        transform.position = startPos;
    }

    Transform FindClosestPlayer()
    {
        PlayerController3D[] players = FindObjectsByType<PlayerController3D>(FindObjectsSortMode.None);

        Transform closest = players[0].transform;
        float minDist = Mathf.Infinity;

        foreach (var p in players)
        {
            float d = Vector3.Distance(transform.position, p.transform.position);
            if (d < minDist)
            {
                minDist = d;
                closest = p.transform;
            }
        }

        return closest;
    }
}