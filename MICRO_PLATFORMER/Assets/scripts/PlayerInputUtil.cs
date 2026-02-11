using UnityEngine;
using UnityEngine.InputSystem;

public static class PlayerInputUtil
{
    public static bool EnsureEnabled(PlayerInput pi)
    {
        if (!pi) return false;
        if (!pi.gameObject.activeInHierarchy) return false;

        // If the component itself is disabled, ActivateInput() won't help
        if (!pi.enabled) pi.enabled = true;

        // Ensure actions asset is enabled
        if (pi.actions != null && !pi.actions.enabled)
            pi.actions.Enable();

        // Ensure PlayerInput is active
        if (!pi.inputIsActive)
            pi.ActivateInput();

        return pi.inputIsActive;
    }

    public static bool SafeSwitchMap(PlayerInput pi, string mapName)
    {
        if (!EnsureEnabled(pi)) return false;
        if (pi.currentActionMap != null && pi.currentActionMap.name == mapName) return true;

        pi.SwitchCurrentActionMap(mapName);
        return true;
    }
}
