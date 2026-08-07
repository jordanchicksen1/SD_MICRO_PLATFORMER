using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static PlayerCombat;

public class PlayerHealthUI : MonoBehaviour
{
    [SerializeField] Image healthFill;
    [SerializeField] TextMeshProUGUI healthText;

    [Header("Key UI")]
    [SerializeField] GameObject keyIcon;   // drag your key icon object here

    [Header("Weapon UI")]
    [SerializeField] GameObject batIcon;
    [SerializeField] GameObject boomerangIcon;
    [SerializeField] GameObject glovesIcon;
    Vector3 iconOriginalScale;
    Coroutine iconAnimation;

    [Header("Reserve Weapon Icons")]
    [SerializeField] GameObject reserveBatIcon;
    [SerializeField] GameObject reserveBoomerangIcon;
    [SerializeField] GameObject reserveGlovesIcon;
    Coroutine reserveIconAnimation;
    public AudioSource reserveSFX;

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

        batIcon.SetActive(false);
        boomerangIcon.SetActive(false);
        glovesIcon.SetActive(false);
        reserveBatIcon.SetActive(false);
        reserveBoomerangIcon.SetActive(false);
        reserveGlovesIcon.SetActive(false);

        iconOriginalScale = batIcon.transform.localScale;
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

    public void SetWeaponIcon(CombatTool weapon)
    {
        if (iconAnimation != null)
            StopCoroutine(iconAnimation);

        GameObject currentIcon = null;

        if (batIcon.activeSelf)
            currentIcon = batIcon;
        else if (boomerangIcon.activeSelf)
            currentIcon = boomerangIcon;
        else if (glovesIcon.activeSelf)
            currentIcon = glovesIcon;

        if (weapon == CombatTool.Kick)
        {
            if (currentIcon != null)
                iconAnimation = StartCoroutine(PopOut(currentIcon));

            return;
        }

        if (currentIcon != null)
            currentIcon.SetActive(false);

        GameObject newIcon = null;

        switch (weapon)
        {
            case CombatTool.BaseballBat:
                newIcon = batIcon;
                break;

            case CombatTool.Boomerang:
                newIcon = boomerangIcon;
                break;

            case CombatTool.BoxingGloves:
                newIcon = glovesIcon;
                break;
        }

        if (newIcon != null)
            iconAnimation = StartCoroutine(PopIn(newIcon));

        
    }

    public void SetReserveWeaponIcon(CombatTool weapon)
    {
        if (reserveIconAnimation != null)
            StopCoroutine(reserveIconAnimation);

        GameObject currentIcon = null;

        if (reserveBatIcon.activeSelf)
            currentIcon = reserveBatIcon;
        else if (reserveBoomerangIcon.activeSelf)
            currentIcon = reserveBoomerangIcon;
        else if (reserveGlovesIcon.activeSelf)
            currentIcon = reserveGlovesIcon;

        if (currentIcon != null)
            currentIcon.SetActive(false);

        GameObject newIcon = null;

        switch (weapon)
        {
            case CombatTool.BaseballBat:
                newIcon = reserveBatIcon;
                break;

            case CombatTool.Boomerang:
                newIcon = reserveBoomerangIcon;
                break;

            case CombatTool.BoxingGloves:
                newIcon = reserveGlovesIcon;
                break;
        }

        if (newIcon != null)
            reserveIconAnimation = StartCoroutine(PopIn(newIcon));

        reserveSFX.Play();
    }

    public void ClearReserveWeaponIcon()
    {
        if (reserveIconAnimation != null)
            StopCoroutine(reserveIconAnimation);

        if (reserveBatIcon.activeSelf)
            reserveIconAnimation = StartCoroutine(PopOut(reserveBatIcon));
        else if (reserveBoomerangIcon.activeSelf)
            reserveIconAnimation = StartCoroutine(PopOut(reserveBoomerangIcon));
        else if (reserveGlovesIcon.activeSelf)
            reserveIconAnimation = StartCoroutine(PopOut(reserveGlovesIcon));
    }



    IEnumerator PopIn(GameObject icon)
    {
        icon.SetActive(true);

        icon.transform.localScale = Vector3.zero;

        float timer = 0f;
        float duration = 0.12f;

        Vector3 overshoot = iconOriginalScale * 1.2f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            icon.transform.localScale =
                Vector3.Lerp(
                    Vector3.zero,
                    overshoot,
                    timer / duration);

            yield return null;
        }

        timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            icon.transform.localScale =
                Vector3.Lerp(
                    overshoot,
                    iconOriginalScale,
                    timer / duration);

            yield return null;
        }

        icon.transform.localScale = iconOriginalScale;
    }

    IEnumerator PopOut(GameObject icon)
    {
        float timer = 0f;
        float duration = 0.12f;

        Vector3 overshoot = iconOriginalScale * 1.2f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            icon.transform.localScale =
                Vector3.Lerp(
                    iconOriginalScale,
                    overshoot,
                    timer / duration);

            yield return null;
        }

        timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            icon.transform.localScale =
                Vector3.Lerp(
                    overshoot,
                    Vector3.zero,
                    timer / duration);

            yield return null;
        }

        icon.SetActive(false);

        icon.transform.localScale = iconOriginalScale;
    }
}
