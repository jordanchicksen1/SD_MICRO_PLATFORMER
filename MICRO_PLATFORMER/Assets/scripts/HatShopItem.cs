using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HatShopItem : MonoBehaviour
{
    [Header("Hat")]
    [SerializeField] HatType hatType;
    [SerializeField] int cost;

    [Header("UI")]
    [SerializeField] Button buyButton;
    [SerializeField] TextMeshProUGUI buttonText;
    [SerializeField] TextMeshProUGUI priceText;
    [SerializeField] HatShopUI shopUI;

    [Header("Audio")]
    [SerializeField] AudioSource purchaseSFX;
    [SerializeField] AudioSource errorSFX;

    void Start()
    {
        Refresh();
    }

    public void BuyHat()
    {
        if (CosmeticManager.Instance.HasPurchased(hatType))
            return;

        if (!CurrencyManager.Instance.SpendCoins(cost))
        {
            if (errorSFX)
                errorSFX.Play();

            if (shopUI)
                shopUI.ShowSpeech(ShopSpeechType.NoMoney);

            return;
        }

        CosmeticManager.Instance.PurchaseHat(hatType);

        HatStandUI stand =
    FindFirstObjectByType<HatStandUI>();

        if (stand != null)
        {
            stand.RefreshPurchasedItems();
        }

        if (purchaseSFX)
            purchaseSFX.Play();

        Refresh();
        if (shopUI)
            shopUI.ShowSpeech(ShopSpeechType.Purchase);

        if (shopUI)
            shopUI.SelectExitButton();
    }

    void Refresh()
    {
        if (priceText)
            priceText.text = cost + " Coins";

        bool purchased =
            CosmeticManager.Instance.HasPurchased(hatType);

        if (purchased)
        {
            buttonText.text = "Purchased";
            buyButton.interactable = false;
        }
        else
        {
            buttonText.text = "Buy";
            buyButton.interactable = true;
        }
    }
}