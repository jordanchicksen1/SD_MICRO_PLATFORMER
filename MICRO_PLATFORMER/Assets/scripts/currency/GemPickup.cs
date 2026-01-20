using UnityEngine;

public class GemPickup : MonoBehaviour
{
    [SerializeField] int amount = 1;

    void OnTriggerEnter(Collider other)
    {
        if (!other.GetComponentInParent<PlayerController3D>())
            return;

        CurrencyManager.Instance.AddGems(amount);
        Destroy(gameObject);
    }
}
