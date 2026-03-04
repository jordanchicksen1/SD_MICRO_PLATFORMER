using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHand : MonoBehaviour
{
    [Header("Attack")]
    [SerializeField] float followSpeed = 3f;
    [SerializeField] float slamHeight = 8f;
    [SerializeField] float slamSpeed = 15f;
    [SerializeField] float telegraphTime = 0.7f;

    Quaternion startRotation;
    Quaternion dazedBaseRotation;

    [Header("Rock")]
    [SerializeField] GameObject rockPrefab;
    [SerializeField] int rockCount = 3;
    [SerializeField] Transform[] rockSpawnPoints;
    [SerializeField] float rockFallHeight = 12f;

    BossController boss;

    Vector3 startPos;
    bool attacking;
    bool stunned;

    List<int> lastUsedIndexes = new List<int>();

    Coroutine slamRoutine;

    [SerializeField] GameObject starPrefab;
    [SerializeField] Transform starOrbitPoint;

    GameObject starOrbitContainer;

    [SerializeField] float dazedWobbleAngle = 20f;
    [SerializeField] float dazedWobbleSpeed = 4f;
    float dazedTimer;

    public bool IsStunned => stunned;

    enum HandState
    {
        Normal,
        Dazed,
        Incapacitated
    }

    HandState currentState = HandState.Normal;

    public bool IsIncapacitated => currentState == HandState.Incapacitated;
    public bool IsDazed => currentState == HandState.Dazed;

    public void SetBoss(BossController b)
    {
        boss = b;
    }

    void Start()
    {
        startPos = transform.position;
        startRotation = Quaternion.identity;
    }

    void Update()
    {
        if (currentState == HandState.Dazed)
        {
            dazedTimer += Time.deltaTime;

            float angle = Mathf.Sin(dazedTimer * dazedWobbleSpeed) * dazedWobbleAngle;

            transform.rotation =
                dazedBaseRotation * Quaternion.AngleAxis(angle, Vector3.forward);
        }
        else if (currentState == HandState.Incapacitated)
        {
            // stay in the pose where the wobble stopped
            transform.rotation = dazedBaseRotation;
        }
    }

    public IEnumerator PerformAttack()
    {
        if (stunned) yield break;

        slamRoutine = StartCoroutine(SlamAttack());
        yield return slamRoutine;
    }



    IEnumerator SlamAttack()
    {
        attacking = true;

        Transform target = FindClosestPlayer();
        Vector3 hoverPos = target.position + Vector3.up * slamHeight;

        // 1?? Move above player (telegraph phase)
        float t = 0f;
        Vector3 initialPos = transform.position;

        while (t < telegraphTime)
        {
            transform.position = Vector3.Lerp(initialPos, hoverPos, t / telegraphTime);
            t += Time.deltaTime;
            yield return null;
        }

        transform.position = hoverPos;

        // 2?? Rotate downward (swat pose)
        Quaternion attackRotation = Quaternion.Euler(-90f, 0f, 0f);

        float rotateTime = 0.15f;
        float r = 0f;

        while (r < rotateTime)
        {
            transform.rotation = Quaternion.Lerp(startRotation, attackRotation, r / rotateTime);
            r += Time.deltaTime;
            yield return null;
        }

        transform.rotation = attackRotation;

        // 3?? Slam down
        while (transform.position.y > target.position.y)
        {
            transform.position += Vector3.down * slamSpeed * Time.deltaTime;
            yield return null;
        }

        // 4?? Check impact
        CheckImpact();
        if (currentState == HandState.Dazed)
        {
            yield break; // stay on ground
        }

        if (stunned)
        {
            attacking = false;
            yield break; // end slam immediately
        }

        // 5?? Return to start
        while (Vector3.Distance(transform.position, startPos) > 0.1f)
        {
            transform.position = Vector3.Lerp(transform.position, startPos, 4f * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, startRotation, 4f * Time.deltaTime);
            yield return null;
        }

        transform.position = startPos;
        transform.rotation = startRotation;

        attacking = false;
    }
    void CheckImpact()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, 2f);

        bool hitRock = false;
        bool hitPlayer = false;

        foreach (var h in hits)
        {
            if (h.CompareTag("Rock"))
            {
                hitRock = true;
                Destroy(h.gameObject);
            }

            if (h.CompareTag("Player"))
            {
                hitPlayer = true;

                PlayerHealth health = h.GetComponent<PlayerHealth>();
                if (health)
                    health.TakeDamage(1, transform.position);
            }
        }

        if (hitRock)
        {

            dazedBaseRotation = transform.rotation;
            currentState = HandState.Dazed;
            boss.OnHandDazed(this);
            return;
        }
        else if (!hitPlayer)
        {
            SpawnRocks(); // Only spawn rocks if it hit ground but NOT player
        }
    }

    void SpawnRocks()
    {
        if (rockSpawnPoints.Length == 0) return;

        List<int> usedThisRound = new List<int>();
        List<int> availableIndexes = new List<int>();

        // Build list of usable spawn points (not used last time)
        for (int i = 0; i < rockSpawnPoints.Length; i++)
        {
            if (!lastUsedIndexes.Contains(i))
                availableIndexes.Add(i);
        }

        // If somehow we blocked too many (edge case), reset memory
        if (availableIndexes.Count < rockCount)
        {
            availableIndexes.Clear();
            for (int i = 0; i < rockSpawnPoints.Length; i++)
                availableIndexes.Add(i);
        }

        for (int i = 0; i < rockCount; i++)
        {
            int randomListIndex = Random.Range(0, availableIndexes.Count);
            int spawnIndex = availableIndexes[randomListIndex];

            usedThisRound.Add(spawnIndex);
            availableIndexes.RemoveAt(randomListIndex);

            Vector3 spawnPos = rockSpawnPoints[spawnIndex].position + Vector3.up * rockFallHeight;

            Instantiate(rockPrefab, spawnPos, Quaternion.identity);
        }

        // Store memory for next slam
        lastUsedIndexes = usedThisRound;
    }


    public void ResetHandState()
    {
        currentState = HandState.Normal;
        transform.position = startPos;
        transform.rotation = startRotation;
        dazedTimer = 0f;

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




    void OnTriggerEnter(Collider other)
    {
        if (currentState != HandState.Dazed) return;

        PlayerController3D player = other.GetComponent<PlayerController3D>();
        if (!player) return;

        Debug.Log("Player touched dazed hand");

        if (player.IsGroundPounding())
        {
            Debug.Log("Hand incapacitated!");

            currentState = HandState.Incapacitated;
            SpawnStars();
            dazedTimer = 0f;
            boss.OnHandIncapacitated(this);
        }
    }

    void SpawnStars()
    {
        if (starOrbitContainer != null) return;

        starOrbitContainer = new GameObject("StarOrbit");
        starOrbitContainer.AddComponent<StarOrbit>();
        starOrbitContainer.transform.SetParent(starOrbitPoint);
        starOrbitContainer.transform.localPosition = Vector3.zero;

        float radius = 0.8f;
        int starCount = 3;

        for (int i = 0; i < starCount; i++)
        {
            float angle = i * Mathf.PI * 2f / starCount;

            Vector3 pos = new Vector3(
                Mathf.Cos(angle) * radius,
                0,
                Mathf.Sin(angle) * radius
            );

            GameObject star = Instantiate(starPrefab, starOrbitContainer.transform);
            star.transform.localPosition = pos;
        }
    }

    public IEnumerator ReturnToStart()
    {
        float t = 0f;
        float moveTime = 0.5f;

        Vector3 start = transform.position;
        Quaternion startRot = transform.rotation;

        while (t < moveTime)
        {
            transform.position = Vector3.Lerp(start, startPos, t / moveTime);
            transform.rotation = Quaternion.Lerp(startRot, startRotation, t / moveTime);
            t += Time.deltaTime;
            yield return null;
        }

        transform.position = startPos;
        transform.rotation = startRotation;

        currentState = HandState.Normal;

        if (starOrbitContainer != null)
        {
            Destroy(starOrbitContainer);
            starOrbitContainer = null;
        }

        dazedTimer = 0f;
    }
}