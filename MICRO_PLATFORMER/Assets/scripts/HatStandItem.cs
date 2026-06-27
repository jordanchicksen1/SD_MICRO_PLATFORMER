using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HatStandItem : MonoBehaviour
{
    [SerializeField] HatType hatType;

    [Header("Buttons")]
    [SerializeField] Button player1Button;
    [SerializeField] Button player2Button;

    [SerializeField] TextMeshProUGUI player1Text;
    [SerializeField] TextMeshProUGUI player2Text;

    void Start()
    {
        Refresh();
    }

    void Refresh()
    {
        player1Text.text = "Equip P1";
        player2Text.text = "Equip P2";
    }
}