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

    int currentHits;
    bool vulnerable;

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

        // TODO: play fall animation

        yield return new WaitForSeconds(stunDuration);

        head.SetVulnerable(false);
        vulnerable = false;

        leftHand.ResetHand();
        rightHand.ResetHand();
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
}