using UnityEngine;

public class HubHatVisual : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] bool playerOne = true;

    [Header("Hats")]
    [SerializeField] GameObject grassyHat;
    [SerializeField] GameObject hardHat;
    [SerializeField] GameObject beanie;

    [Header("Effects")]
    [SerializeField] GameObject poofPrefab;
    [SerializeField] Transform poofSpawnPoint;
    [SerializeField] float poofLifetime = 2f;

    void OnEnable()
    {
        RefreshHat();
    }

    public void RefreshHat()
    {
        grassyHat.SetActive(false);
        hardHat.SetActive(false);
        beanie.SetActive(false);

        HatType hat =
            playerOne
            ? CosmeticManager.Instance.Player1Hat
            : CosmeticManager.Instance.Player2Hat;

        if (poofPrefab && poofSpawnPoint)
        {
            GameObject poof =
                Instantiate(
                    poofPrefab,
                    poofSpawnPoint.position,
                    Quaternion.identity);

            Destroy(poof, poofLifetime);
        }

        switch (hat)
        {
            case HatType.GrassyHat:
                grassyHat.SetActive(true);
                break;

            case HatType.HardHat:
                hardHat.SetActive(true);
                break;

            case HatType.Beanie:
                beanie.SetActive(true);
                break;
        }
    }
}