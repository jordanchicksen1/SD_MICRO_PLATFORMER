using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] int maxHealth = 3;
    public int MaxHealth => maxHealth;

    int currentHealth;
    public int CurrentHealth => currentHealth;

    public bool IsInvulnerable { get; private set; }

    [SerializeField] float invulnerabilityTime = 1f;
    public float InvulnerabilityDuration => invulnerabilityTime;

    public event Action<int> OnHealthChanged;
    public event Action<Vector3> OnDamaged;

    
    public event Action OnDied;
    public bool hasDied;

    public AudioSource hurtSFX;

    void Awake()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth);
    }

    public void TakeDamage(int amount, Vector3 sourcePosition)
    {
        if (IsInvulnerable) return;
        if (hasDied) return;

        

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log($"[{name}] Took damage. HP now: {currentHealth}");

        OnHealthChanged?.Invoke(currentHealth);
        OnDamaged?.Invoke(sourcePosition);
        hurtSFX.Play();
        if (currentHealth <= 0)
        {
            Debug.Log($"[{name}] DIED -> OnDied invoked");
            hasDied = true;
            OnDied?.Invoke();
            return;
        }

        StartCoroutine(InvulnerabilityCoroutine());
    }

    public void Heal(int amount)
    {
        if (hasDied) return;

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        OnHealthChanged?.Invoke(currentHealth);
    }

    // NEW: used by bubble revive
    public void ReviveTo(int health)
    {
        
        hasDied = false;
        currentHealth = Mathf.Clamp(health, 1, maxHealth);
        OnHealthChanged?.Invoke(currentHealth);
    }

    System.Collections.IEnumerator InvulnerabilityCoroutine()
    {
        IsInvulnerable = true;
        yield return new WaitForSeconds(invulnerabilityTime);
        IsInvulnerable = false;
    }
}
