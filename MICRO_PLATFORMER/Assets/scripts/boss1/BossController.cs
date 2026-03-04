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

    [Header("Fight Settings")]
    [SerializeField] int hitsToDefeat = 3;
    [SerializeField] float stunDuration = 4f;

    Coroutine attackRoutine;
    bool useLeftHand = true;

    int currentHits;
    bool vulnerable;

    bool fightStarted;

    Vector3 startPos;
    Quaternion startRot;

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
            // If both hands are down, trigger boss vulnerability
            if (leftHand.IsIncapacitated && rightHand.IsIncapacitated)
            {
                Debug.Log("Both hands incapacitated! Boss falling.");
                vulnerabilityRoutine = StartCoroutine(VulnerabilityPhase());
                yield break;
            }

            // LEFT HAND
            if (!leftHand.IsIncapacitated && !leftHand.IsDazed)
            {
                yield return leftHand.PerformAttack();
            }

            yield return new WaitForSeconds(1.2f);

            // Check again in case player incapacitated the other hand
            if (leftHand.IsIncapacitated && rightHand.IsIncapacitated)
            {
                Debug.Log("Both hands incapacitated! Boss falling.");
                StartCoroutine(VulnerabilityPhase());
                yield break;
            }

            // RIGHT HAND
            if (!rightHand.IsIncapacitated && !rightHand.IsDazed)
            {
                yield return rightHand.PerformAttack();
            }

            yield return new WaitForSeconds(1.2f);
        }
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

    

    IEnumerator VulnerabilityPhase()
    {
        Debug.Log("Boss entering vulnerability phase");
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
        if (!vulnerable) return;

        currentHits++;

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
    }

   


    void Die()
    {
        Debug.Log("Boss defeated!");
        // Play death animation
        // Trigger door open
        // Victory cutscene
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