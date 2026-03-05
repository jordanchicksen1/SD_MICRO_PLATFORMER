using UnityEngine;
using System.Collections;

public class BossController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] BossHand leftHand;
    [SerializeField] BossHand rightHand;
    [SerializeField] BossHead head;
    Transform headTransform;
    Vector3 headStartPos;
    Quaternion headStartRot;
    Coroutine vulnerabilityRoutine;
    [SerializeField] Transform player1ResetPoint;
    [SerializeField] Transform player2ResetPoint;
    [SerializeField] Renderer[] bossRenderers;
    [SerializeField] Material[] damageMaterials;
    [SerializeField] Renderer[] deathRenderers;
    [SerializeField] GameObject deathVFX;
    [SerializeField] GameObject gemPrefab;
    [SerializeField] Transform gemSpawnPoint;
    [SerializeField] GameObject x1;
    [SerializeField] GameObject x2;
    [SerializeField] GameObject x3;
    Transform currentTarget;
    bool bossDead = false;
    [SerializeField] BossGateSequence gateSequence;

    [Header("Fight Settings")]
    [SerializeField] int hitsToDefeat = 3;
    [SerializeField] float stunDuration = 4f;

    Coroutine attackRoutine;
    bool vulnerabilityActive = false;
    

    int currentHits;
    bool vulnerable;

    bool fightStarted;

    Vector3 startPos;
    Quaternion startRot;

    public IEnumerator IntroAnimation()
    {
        float duration = 1.2f;
        float t = 0f;

        Quaternion leftStart = leftHand.transform.localRotation;
        Quaternion rightStart = rightHand.transform.localRotation;

        Quaternion leftUp = leftStart * Quaternion.Euler(-40f, 0f, 0f);
        Quaternion rightUp = rightStart * Quaternion.Euler(-40f, 0f, 0f);

        // Raise hands
        while (t < duration)
        {
            float a = t / duration;

            leftHand.transform.localRotation =
                Quaternion.Lerp(leftStart, leftUp, a);

            rightHand.transform.localRotation =
                Quaternion.Lerp(rightStart, rightUp, a);

            t += Time.deltaTime;
            yield return null;
            CameraShake.Shake(0.2f, 0.13f);
        }

        yield return new WaitForSeconds(0.6f);

        // Lower hands again
        t = 0f;

        while (t < duration)
        {
            float a = t / duration;

            leftHand.transform.localRotation =
                Quaternion.Lerp(leftUp, leftStart, a);

            rightHand.transform.localRotation =
                Quaternion.Lerp(rightUp, rightStart, a);

            t += Time.deltaTime;
            yield return null;
        }
    }

    public IEnumerator MovePlayersToArenaStart()
    {
        PlayerController3D[] players =
            FindObjectsByType<PlayerController3D>(FindObjectsSortMode.None);

        float moveTime = 0.35f;
        float moveT = 0f;

        Vector3[] startPositions = new Vector3[players.Length];
        Vector3[] targetPositions = new Vector3[players.Length];

        for (int i = 0; i < players.Length; i++)
        {
            startPositions[i] = players[i].transform.position;

            if (i == 0)
                targetPositions[i] = player1ResetPoint.position;
            else
                targetPositions[i] = player2ResetPoint.position;
        }

        while (moveT < moveTime)
        {
            for (int i = 0; i < players.Length; i++)
            {
                players[i].transform.position =
                    Vector3.Lerp(startPositions[i], targetPositions[i], moveT / moveTime);
            }

            moveT += Time.deltaTime;
            yield return null;
        }
    }

    public void StartFight()
    {
        if (fightStarted) return;
        fightStarted = true;

        attackRoutine = StartCoroutine(AttackPattern());

        Debug.Log("Boss fight started!");
    }

    IEnumerator AttackPattern()
    {
        while (true)
        {
            // If both hands are down, start boss vulnerability
            if (leftHand.IsIncapacitated && rightHand.IsIncapacitated)
            {
                vulnerabilityRoutine = StartCoroutine(VulnerabilityPhase());
                yield break;
            }

            // LEFT HAND ATTACK
            if (!leftHand.IsIncapacitated && !leftHand.IsDazed)
            {
                yield return leftHand.PerformAttack();
            }

            yield return new WaitForSeconds(1.2f);

            // Check again before right hand attacks
            if (leftHand.IsIncapacitated && rightHand.IsIncapacitated)
            {
                vulnerabilityRoutine = StartCoroutine(VulnerabilityPhase());
                yield break;
            }

            // RIGHT HAND ATTACK
            if (!rightHand.IsIncapacitated && !rightHand.IsDazed)
            {
                yield return rightHand.PerformAttack();
            }

            yield return new WaitForSeconds(1.2f);
        }
    }

    public void SetTarget(Transform target)
    {
        currentTarget = target;
    }
    void Start()
    {
        leftHand.SetBoss(this);
        rightHand.SetBoss(this);
        head.SetBoss(this);
        startPos = transform.position;
        startRot = transform.rotation;
        headTransform = head.transform;
        headStartPos = headTransform.localPosition;
        headStartRot = headTransform.localRotation;
    }

    void Update()
    {
        if (currentTarget == null) return;
        if (vulnerable) return; // don't rotate while boss is knocked down

        Vector3 direction = currentTarget.position - headTransform.position;
        direction.y = 0;

        Quaternion targetRot = Quaternion.LookRotation(-direction);

        headTransform.rotation =
            Quaternion.Lerp(headTransform.rotation, targetRot, 3f * Time.deltaTime);
    }

    IEnumerator VulnerabilityPhase()
    {
        Debug.Log("Boss entering vulnerability phase");
        vulnerabilityActive = true;
        vulnerable = true;
        head.SetVulnerable(true);

        if (attackRoutine != null)
            StopCoroutine(attackRoutine);

        DestroyAllRocks();

        // ---- FALL ANIMATION ----
        Vector3 fallTargetPos = headStartPos + Vector3.down * 2f;
        Quaternion fallTargetRot = headStartRot * Quaternion.Euler(-90f, 0f, 0f);

        float t = 0f;
        float fallTime = 0.4f;

        while (t < fallTime)
        {
            headTransform.localPosition = Vector3.Lerp(headStartPos, fallTargetPos, t / fallTime);
            headTransform.localRotation = Quaternion.Lerp(headStartRot, fallTargetRot, t / fallTime);
            t += Time.deltaTime;
            yield return null;
        }

        headTransform.localPosition = fallTargetPos;
        headTransform.localRotation = fallTargetRot;

        // ---- STUN WINDOW ----
        yield return new WaitForSeconds(10f);

        // ---- STAND BACK UP ----
        t = 0f;

        while (t < fallTime)
        {
            headTransform.localPosition = Vector3.Lerp(fallTargetPos, headStartPos, t / fallTime);
            headTransform.localRotation = Quaternion.Lerp(fallTargetRot, headStartRot, t / fallTime);
            t += Time.deltaTime;
            yield return null;
        }

        headTransform.localPosition = headStartPos;
        headTransform.localRotation = headStartRot;

        head.SetVulnerable(false);
        vulnerable = false;

        leftHand.ResetHandState();
        rightHand.ResetHandState();

        attackRoutine = StartCoroutine(AttackPattern());
    }

    void DestroyAllRocks()
    {
        GameObject[] rocks = GameObject.FindGameObjectsWithTag("Rock");

        foreach (var r in rocks)
        {
            Destroy(r);
        }
    }

    public void DamageBoss()
    {
        if (!vulnerable || bossDead || !vulnerabilityActive) return;

        currentHits++;
        UpdateBossDamageVisual();
        StartCoroutine(HitFlash());

        Debug.Log("Boss hit! Total hits: " + currentHits);

        if (vulnerabilityRoutine != null)
            StopCoroutine(vulnerabilityRoutine);

        if (currentHits >= hitsToDefeat)
        {
            Die();
            return;
        }

        StartCoroutine(ResetBoss());
        
    }


    void UpdateBossDamageVisual()
    {
        int index = Mathf.Clamp(currentHits, 0, damageMaterials.Length - 1);

        foreach (Renderer r in bossRenderers)
        {
            r.material = damageMaterials[index];
        }
    }

    IEnumerator HitFlash()
    {
        // Store the current material index
        int index = Mathf.Clamp(currentHits, 0, damageMaterials.Length - 1);

        // Flash white
        foreach (Renderer r in bossRenderers)
        {
            r.material.color = Color.white;
        }

        yield return new WaitForSeconds(0.08f);

        // Restore correct damage material
        foreach (Renderer r in bossRenderers)
        {
            r.material = damageMaterials[index];
        }
    }
    IEnumerator ResetBoss()
    {
        yield return new WaitForSeconds(0.5f);
        vulnerable = false;
        head.SetVulnerable(false);

        // Teleport players
        PlayerController3D[] players = FindObjectsByType<PlayerController3D>(FindObjectsSortMode.None);

        float moveTime = 0.35f;
        float moveT = 0f;

        Vector3[] startPositions = new Vector3[players.Length];
        Vector3[] targetPositions = new Vector3[players.Length];

        for (int i = 0; i < players.Length; i++)
        {
            startPositions[i] = players[i].transform.position;

            if (i == 0)
                targetPositions[i] = player1ResetPoint.position;
            else
                targetPositions[i] = player2ResetPoint.position;
        }

        while (moveT < moveTime)
        {
            for (int i = 0; i < players.Length; i++)
            {
                players[i].transform.position =
                    Vector3.Lerp(startPositions[i], targetPositions[i], moveT / moveTime);
            }

            moveT += Time.deltaTime;
            yield return null;
        }

        // Move hands back smoothly
        StartCoroutine(leftHand.ReturnToStart());
        StartCoroutine(rightHand.ReturnToStart());

        // Rotate boss head upright
        float t = 0f;
        float resetTime = 0.6f;

        Vector3 startPos = headTransform.localPosition;
        Quaternion startRot = headTransform.localRotation;

        while (t < resetTime)
        {
            headTransform.localPosition =
                Vector3.Lerp(startPos, headStartPos, t / resetTime);

            headTransform.localRotation =
                Quaternion.Lerp(startRot, headStartRot, t / resetTime);

            t += Time.deltaTime;
            yield return null;
        }

        headTransform.localPosition = headStartPos;
        headTransform.localRotation = headStartRot;

        yield return new WaitForSeconds(0.5f);

        attackRoutine = StartCoroutine(AttackPattern());
        vulnerabilityActive = false;
    }




    void Die()
    {
        if (bossDead) return;

        bossDead = true;

        Debug.Log("Boss defeated!");

        StartCoroutine(DeathSequence());
    }

    IEnumerator DeathSequence()
    {
        // stop attacks
        if (attackRoutine != null)
            StopCoroutine(attackRoutine);

        DestroyAllRocks();
        x1.SetActive(false);
        x2.SetActive(false);
        x3.SetActive(false);

        // spawn the smoke effect
        GameObject smoke = Instantiate(deathVFX, head.transform.position, Quaternion.identity);


        // hide the boss meshes so it looks like it disappeared
        foreach (Renderer r in deathRenderers)
        {
            r.enabled = false;
        }

        // wait so the smoke can play
        yield return new WaitForSeconds(0.8f);

        // spawn the gem
        // start gate + gem sequence
        if (gateSequence != null)
        {
            gateSequence.StartSequence();
        }

        // destroy boss
        Destroy(gameObject);

        // destroy boss controller
        Destroy(gameObject);
    }

    public void OnHandDazed(BossHand hand)
    {
        // Do nothing special yet
    }

    public void OnHandIncapacitated(BossHand hand)
    {
        Debug.Log("Hand incapacitated: " + hand.name);

        Debug.Log("Left incapacitated: " + leftHand.IsIncapacitated);
        Debug.Log("Right incapacitated: " + rightHand.IsIncapacitated);

       
    }
}