using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    [SerializeField] int maxHealth = 1;
    [SerializeField] GameObject deathEffect;

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

    IEnumerator DieDelayed()
    {
        yield return new WaitForSeconds(1f);
        deathEffect.SetActive(true);
        if (deathEffect)
            Instantiate(deathEffect, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}
