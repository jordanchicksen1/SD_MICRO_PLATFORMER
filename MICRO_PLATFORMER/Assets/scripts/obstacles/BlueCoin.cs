using UnityEngine;

public class BlueCoin : MonoBehaviour
{
    BlueCoinChallenge manager;

    public void SetManager(BlueCoinChallenge m)
    {
        manager = m;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.GetComponentInParent<PlayerController3D>())
            return;

        manager?.RegisterCoinCollected();
        Destroy(gameObject);
    }
}
