using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHealthUI : MonoBehaviour
{
    [SerializeField] Image healthFill;
    [SerializeField] TextMeshProUGUI healthText;

    [Header("Key UI")]
    [SerializeField] GameObject keyIcon;   // drag your key icon object here

    PlayerHealth playerHealth;
    int maxHealth;

    public PlayerHealth BoundHealth => playerHealth;

    public void Bind(PlayerHealth health)
    {
        playerHealth = health;
        maxHealth = health.MaxHealth;

        playerHealth.OnHealthChanged += UpdateUI;
        UpdateUI(playerHealth.CurrentHealth);

        // start hidden
        if (keyIcon != null)
            keyIcon.SetActive(false);
    }

    public void SetHasKey(bool hasKey)
    {
        if (keyIcon != null)
            keyIcon.SetActive(hasKey);
    }

    void UpdateUI(int current)
    {
        healthFill.fillAmount = (float)current / maxHealth;

        if (healthText != null)
            healthText.text = current.ToString();
    }
}
