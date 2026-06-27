using System.Collections.Generic;
using UnityEngine;

public class CosmeticManager : MonoBehaviour
{
    public static CosmeticManager Instance { get; private set; }

    const string PurchasedKey = "PurchasedHats";
    const string Player1HatKey = "Player1Hat";
    const string Player2HatKey = "Player2Hat";

    HashSet<HatType> purchasedHats = new();

    public HatType Player1Hat { get; private set; } = HatType.None;
    public HatType Player2Hat { get; private set; } = HatType.None;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        Load();
    }

    public void Start()
    {
        //ResetCosmetics();
    }

    void Load()
    {
        purchasedHats.Clear();

        string purchased = PlayerPrefs.GetString(PurchasedKey, "");

        if (!string.IsNullOrEmpty(purchased))
        {
            string[] hats = purchased.Split(',');

            foreach (string hat in hats)
            {
                if (System.Enum.TryParse(hat, out HatType type))
                    purchasedHats.Add(type);
            }
        }

        Player1Hat =
            (HatType)PlayerPrefs.GetInt(Player1HatKey, (int)HatType.None);

        Player2Hat =
            (HatType)PlayerPrefs.GetInt(Player2HatKey, (int)HatType.None);
    }

    void Save()
    {
        string purchased = string.Join(",", purchasedHats);

        PlayerPrefs.SetString(PurchasedKey, purchased);

        PlayerPrefs.SetInt(Player1HatKey, (int)Player1Hat);

        PlayerPrefs.SetInt(Player2HatKey, (int)Player2Hat);

        PlayerPrefs.Save();
    }

    public bool HasPurchased(HatType hat)
    {
        return purchasedHats.Contains(hat);
    }

    public void PurchaseHat(HatType hat)
    {
        purchasedHats.Add(hat);
        Save();
    }

    public void EquipHat(int playerNumber, HatType hat)
    {
        if (!HasPurchased(hat))
            return;

        if (playerNumber == 1)
            Player1Hat = hat;
        else
            Player2Hat = hat;

        Save();
    }

    public void ResetCosmetics()
    {
        PlayerPrefs.DeleteKey("PurchasedHats");
        PlayerPrefs.DeleteKey("Player1Hat");
        PlayerPrefs.DeleteKey("Player2Hat");

        purchasedHats.Clear();

        Player1Hat = HatType.None;
        Player2Hat = HatType.None;
    }

    public void SetPlayer1Hat(HatType hat)
    {
        Player1Hat = hat;
    }

    public void SetPlayer2Hat(HatType hat)
    {
        Player2Hat = hat;
    }

}