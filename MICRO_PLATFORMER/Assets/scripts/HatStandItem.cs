using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HatStandItem : MonoBehaviour
{
    [SerializeField] HatType hatType;

    [Header("Buttons")]
    [SerializeField] Button player1Button;
    [SerializeField] Button player2Button;
    [SerializeField] HatStandUI hatStandUI;
    [SerializeField] TextMeshProUGUI player1Text;
    [SerializeField] TextMeshProUGUI player2Text;

    [Header("Visuals")]
    [SerializeField] HubHatVisual player1Visual;
    [SerializeField] HubHatVisual player2Visual;

    void Start()
    {
        player1Button.onClick.AddListener(EquipPlayer1);
        player2Button.onClick.AddListener(EquipPlayer2);

        Refresh();
    }

    void EquipPlayer1()
    {
        if (CosmeticManager.Instance.Player1Hat == hatType)
        {
            CosmeticManager.Instance.SetPlayer1Hat(HatType.None);
        }
        else
        {
            CosmeticManager.Instance.SetPlayer1Hat(hatType);
        }

        player1Visual.RefreshHat();

        hatStandUI.RefreshAllItems();
    }

    void EquipPlayer2()
    {
        if (CosmeticManager.Instance.Player2Hat == hatType)
        {
            CosmeticManager.Instance.SetPlayer2Hat(HatType.None);
        }
        else
        {
            CosmeticManager.Instance.SetPlayer2Hat(hatType);
        }

        player2Visual.RefreshHat();

        hatStandUI.RefreshAllItems();
    }

    public void Refresh()
    {
        player1Text.text =
            CosmeticManager.Instance.Player1Hat == hatType
            ? "Unequip P1"
            : "Equip P1";

        player2Text.text =
            CosmeticManager.Instance.Player2Hat == hatType
            ? "Unequip P2"
            : "Equip P2";
    }

    public void RefreshPurchasedState()
    {
        bool purchased = CosmeticManager.Instance.HasPurchased(hatType);

        gameObject.SetActive(purchased);
    }
}