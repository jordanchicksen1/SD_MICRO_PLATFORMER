using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHealthUI : MonoBehaviour
{
    [SerializeField] Image healthFill;
    [SerializeField] TextMeshProUGUI healthText;

    PlayerHealth playerHealth;

    public void Bind(PlayerHealth health)
    {
        playerHealth = health;

        playerHealth.OnHealthChanged += UpdateUI;
        UpdateUI(playerHealth.CurrentHealth, playerHealth.MaxHealth);
    }

    void UpdateUI(int current, int max)
    {
        healthFill.fillAmount = (float)current / max;

        if (healthText != null)
            healthText.text = current.ToString();
    }
}
