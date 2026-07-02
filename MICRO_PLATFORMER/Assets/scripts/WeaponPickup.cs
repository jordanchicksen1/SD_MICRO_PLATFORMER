using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    [SerializeField]
    PlayerCombat.CombatTool weapon;

    private void OnTriggerEnter(Collider other)
    {
        PlayerCombat combat =
            other.GetComponent<PlayerCombat>();

        if (combat == null)
            return;

        combat.SetCombatTool(weapon);

        Destroy(gameObject);
    }
}