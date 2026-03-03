using UnityEngine;
using System.Collections;

public class BossController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] BossHand leftHand;
    [SerializeField] BossHand rightHand;
    [SerializeField] BossHead head;

    [Header("Fight Settings")]
    [SerializeField] int hitsToDefeat = 3;
    [SerializeField] float stunDuration = 4f;

    Coroutine attackRoutine;
    bool useLeftHand = true;

    int currentHits;
    bool vulnerable;

    bool fightStarted;

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
            if (leftHand.IsIncapacitated && rightHand.IsIncapacitated)
            {
                yield return null;
                continue;
            }

            if (!leftHand.IsIncapacitated && !leftHand.IsDazed)
            {
                yield return leftHand.PerformAttack();
            }

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
    }

    public void OnHandStunned()
    {
        if (leftHand.IsStunned && rightHand.IsStunned)
        {
            StartCoroutine(VulnerabilityPhase());
        }
    }

    IEnumerator VulnerabilityPhase()
    {
        vulnerable = true;
        head.SetVulnerable(true);

        // Stop boss attack pattern
        if (attackRoutine != null)
            StopCoroutine(attackRoutine);

        DestroyAllRocks();

        // TODO: play fall animation here

        yield return new WaitForSeconds(15f);

        head.SetVulnerable(false);
        vulnerable = false;

        leftHand.ResetHand();
        rightHand.ResetHand();

        // Restart alternating attacks
        attackRoutine = StartCoroutine(AttackPattern());
        leftHand.ResetHandState();
        rightHand.ResetHandState();
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

        if (currentHits >= hitsToDefeat)
        {
            Die();
        }
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
        if (leftHand.IsIncapacitated && rightHand.IsIncapacitated)
        {
            StartCoroutine(VulnerabilityPhase());
        }
    }
}