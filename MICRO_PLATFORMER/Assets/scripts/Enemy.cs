using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] int maxHealth = 1;
    [SerializeField] GameObject deathEffect;

    int currentHealth;

    Collider enemyCollider;

    void Awake()
    {
        currentHealth = maxHealth;
        enemyCollider = GetComponent<Collider>();
    }


    public void TakeHit()
    {
        if (enemyCollider)
            enemyCollider.enabled = false;

        currentHealth--;

        if (currentHealth <= 0)
            Die();
    }


    void Die()
    {
        if (deathEffect)
            Instantiate(deathEffect, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}
