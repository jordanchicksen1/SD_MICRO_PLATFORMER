using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] int maxHealth = 3;
    int currentHealth;

    public int MaxHealth => maxHealth;
    public int CurrentHealth => currentHealth;

    public System.Action<int, int> OnHealthChanged;
    public System.Action OnDeath;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        currentHealth = Mathf.Max(currentHealth - amount, 0);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth == 0)
            OnDeath?.Invoke();
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }
}
