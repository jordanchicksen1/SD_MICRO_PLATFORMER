using UnityEngine;

public class HatTester : MonoBehaviour
{
    void Start()
    {
        CosmeticManager.Instance.PurchaseHat(HatType.GrassyHat);

        CosmeticManager.Instance.EquipHat(1, HatType.GrassyHat);

        Debug.Log(
            CosmeticManager.Instance.HasPurchased(HatType.GrassyHat)
        );

        Debug.Log(
            CosmeticManager.Instance.Player1Hat
        );
    }
}