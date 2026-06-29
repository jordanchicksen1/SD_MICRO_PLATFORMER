using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    public enum CombatTool
    {
        Kick,
        BaseballBat,
        Boomerang,
        BoxingGloves
    }

    [SerializeField]
    CombatTool currentTool = CombatTool.Kick;

    public CombatTool CurrentTool => currentTool;

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        switch (currentTool)
        {
            case CombatTool.Kick:
                Debug.Log("Kick!");
                break;

            case CombatTool.BaseballBat:
                Debug.Log("Bat!");
                break;

            case CombatTool.Boomerang:
                Debug.Log("Boomerang!");
                break;

            case CombatTool.BoxingGloves:
                Debug.Log("Punch!");
                break;
        }
    }
}