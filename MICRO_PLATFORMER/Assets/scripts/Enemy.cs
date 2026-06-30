using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] int maxHealth = 1;
    [SerializeField] GameObject deathEffect;
    [SerializeField] GameObject coinPrefab;

    [Header("Kick")]
    [SerializeField] float kickForce = 18f;
    [SerializeField] float kickUpForce = 6f;
    [SerializeField] float kickSpinForce = 20f;
    [SerializeField] AudioSource kickSFX;

    [Header("Enemy Stuff")]
    [SerializeField] Transform visuals;

    int currentHealth;
    bool isDead;

    public bool IsDead => isDead;

    void Awake()
    {
        currentHealth = maxHealth;
        deathEffect.SetActive(false);
    }

    void DisableAI()
    {
        EnemyChaser chaser = GetComponent<EnemyChaser>();
        if (chaser != null)
            chaser.enabled = false;

        EnemyJumper jumper = GetComponent<EnemyJumper>();
        if (jumper != null)
            jumper.enabled = false;

        EnemyShooter shooter = GetComponent<EnemyShooter>();
        if (shooter != null)
            shooter.enabled = false;
    }

    public void TakeHit()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
            rb.linearVelocity = Vector3.zero;


        if (isDead)
            return;

        isDead = true;
        DisableAI();

        EnemySquash squash = GetComponentInChildren<EnemySquash>();
        if (squash != null)
            squash.PlaySquash();

        StartCoroutine(DieDelayed());
    }

    public void TakeKick(Vector3 direction)
    {
        Rigidbody rb = GetComponent<Rigidbody>();

        if (rb != null)
            rb.linearVelocity = Vector3.zero;

        if (isDead)
            return;

        isDead = true;
        if (kickSFX != null)
            kickSFX.Play();
        DisableAI();

        if (rb != null)
        {
            Vector3 launch =
                direction.normalized * kickForce;

            launch.y = kickUpForce;

            rb.AddForce(
                launch,
                ForceMode.Impulse);

            StartCoroutine(SpinWhileFlying());
        }

        StartCoroutine(DieDelayed());
    }

    IEnumerator DieDelayed()
    {
        yield return new WaitForSeconds(1f);
        deathEffect.SetActive(true);
        if (deathEffect)
            Instantiate(deathEffect, transform.position, Quaternion.identity);

        if (coinPrefab != null)
        {
            Instantiate(
                coinPrefab,
                transform.position,
                Quaternion.identity);
        }

        Destroy(gameObject);
    }

    IEnumerator SpinWhileFlying()
    {
        while (!isDead)
            yield break;

        while (true)
        {
            visuals.Rotate(
                540f * Time.deltaTime,
                0f,
                0f,
                Space.Self);

            yield return null;
        }
    }
}
