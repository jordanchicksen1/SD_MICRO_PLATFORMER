using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] int maxHealth = 1;
    [SerializeField] GameObject deathEffect;

    [Header("Kick")]
    [SerializeField] float kickForce = 18f;
    [SerializeField] float kickUpForce = 6f;

    int currentHealth;
    bool isDead;

    public bool IsDead => isDead;

    void Awake()
    {
        currentHealth = maxHealth;
        deathEffect.SetActive(false);
    }

    public void TakeHit()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
            rb.linearVelocity = Vector3.zero;


        if (isDead)
            return;

        isDead = true;

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

        if (rb != null)
        {
            Vector3 launch =
                direction.normalized * kickForce;

            launch.y = kickUpForce;

            rb.AddForce(
                launch,
                ForceMode.Impulse);
        }

        StartCoroutine(DieDelayed());
    }

    IEnumerator DieDelayed()
    {
        yield return new WaitForSeconds(1f);
        deathEffect.SetActive(true);
        if (deathEffect)
            Instantiate(deathEffect, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}
