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

    void Start()
    {
        Refresh();
    }

    public void BuyHat()
    {
        if (CosmeticManager.Instance.HasPurchased(hatType))
            return;

        if (!CurrencyManager.Instance.SpendCoins(cost))
            return;

        CosmeticManager.Instance.PurchaseHat(hatType);

        Refresh();
    }

    void Refresh()
    {
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