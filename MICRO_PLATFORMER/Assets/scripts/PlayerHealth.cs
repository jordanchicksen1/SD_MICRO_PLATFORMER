using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] int maxHealth = 5;
    public int MaxHealth => maxHealth;

    int currentHealth;
    public int CurrentHealth => currentHealth;

    public bool IsInvulnerable { get; private set; }

    [SerializeField] float invulnerabilityTime = 1f;

    public event Action<int> OnHealthChanged;
    public event Action<Vector3> OnDamaged;

    void Awake()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth);
    }

    public void TakeDamage(int amount, Vector3 sourcePosition)
    {
        if (IsInvulnerable)
            return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        OnHealthChanged?.Invoke(currentHealth);
        OnDamaged?.Invoke(sourcePosition);

        StartCoroutine(InvulnerabilityCoroutine());
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        OnHealthChanged?.Invoke(currentHealth);
    }

    System.Collections.IEnumerator InvulnerabilityCoroutine()
    {
        IsInvulnerable = true;
        yield return new WaitForSeconds(invulnerabilityTime);
        IsInvulnerable = false;
    }
}
