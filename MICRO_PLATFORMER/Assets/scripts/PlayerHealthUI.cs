using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHealthUI : MonoBehaviour
{
    [SerializeField] Image healthFill;
    [SerializeField] TextMeshProUGUI healthText;

    PlayerHealth playerHealth;
    int maxHealth;

    public void Bind(PlayerHealth health)
    {
        playerHealth = health;
        maxHealth = health.MaxHealth;

        playerHealth.OnHealthChanged += UpdateUI;
        UpdateUI(playerHealth.CurrentHealth);
    }

    void UpdateUI(int current)
    {
        healthFill.fillAmount = (float)current / maxHealth;

        if (healthText != null)
            healthText.text = current.ToString();
    }
}

