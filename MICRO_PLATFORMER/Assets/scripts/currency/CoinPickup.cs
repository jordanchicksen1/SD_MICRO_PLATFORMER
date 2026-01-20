using UnityEngine;

public class CoinPickup : MonoBehaviour
{
    [SerializeField] int amount = 1;

    void OnTriggerEnter(Collider other)
    {
        if (!other.GetComponentInParent<PlayerController3D>())
            return;

        CurrencyManager.Instance.AddCoins(amount);
        Destroy(gameObject);
    }
}
