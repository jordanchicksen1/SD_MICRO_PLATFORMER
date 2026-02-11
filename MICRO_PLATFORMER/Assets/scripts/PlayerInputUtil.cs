using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public static class PlayerInputUtil
{
    // Remember what map each player was on before UI
    static readonly Dictionary<PlayerInput, string> prevMap = new();

    public static List<PlayerInput> GetAllActivePlayerInputs()
    {
        var found = Object.FindObjectsByType<PlayerInput>(FindObjectsSortMode.None);
        List<PlayerInput> list = new();
        foreach (var pi in found)
        {
            if (!pi) continue;
            if (!pi.enabled) continue;
            if (!pi.gameObject.activeInHierarchy) continue;
            list.Add(pi);
        }
        return list;
    }

    public static void EnsureInputEnabled(PlayerInput pi)
    {
        if (!pi) return;
        if (!pi.enabled) return;
        if (!pi.gameObject.activeInHierarchy) return;

        // ActivateInput() only works when the PlayerInput is active in hierarchy
        if (!pi.inputIsActive)
            pi.ActivateInput();
    }

    public static void SafeSwitchMap(PlayerInput pi, string mapName)
    {
        if (!pi) return;
        if (string.IsNullOrEmpty(mapName)) return;
        if (!pi.enabled) return;
        if (!pi.gameObject.activeInHierarchy) return;

        EnsureInputEnabled(pi);

        // If current map is null, SwitchCurrentActionMap can throw if input isn't active.
        // EnsureInputEnabled above should prevent that.
        if (pi.currentActionMap == null || pi.currentActionMap.name != mapName)
            pi.SwitchCurrentActionMap(mapName);
    }

    /// <summary>
    /// Switch ALL active players to UI map (and remember previous maps).
    /// </summary>
    public static void EnterUIMode(string uiMapName)
    {
        var players = GetAllActivePlayerInputs();
        foreach (var pi in players)
        {
            if (!prevMap.ContainsKey(pi))
            {
                string old = (pi.currentActionMap != null) ? pi.currentActionMap.name : null;
                prevMap[pi] = old; // can be null, that's ok
            }

            SafeSwitchMap(pi, uiMapName);
        }
    }

    /// <summary>
    /// Restore each player to the map they had before UI.
    /// If we don’t know it, fall back to gameplayMapName.
    /// </summary>
    public static void ExitUIMode(string gameplayMapName)
    {
        var players = GetAllActivePlayerInputs();
        foreach (var pi in players)
        {
            string restore = gameplayMapName;

            if (prevMap.TryGetValue(pi, out var old) && !string.IsNullOrEmpty(old))
                restore = old;

            SafeSwitchMap(pi, restore);
        }

        // Clean up any destroyed PlayerInputs from the dictionary
        CleanupPrevMap();
    }

    static void CleanupPrevMap()
    {
        var dead = new List<PlayerInput>();
        foreach (var kv in prevMap)
        {
            if (!kv.Key) dead.Add(kv.Key);
        }
        foreach (var k in dead)
            prevMap.Remove(k);
    }

    /// <summary>
    /// Use this when a menu “takes over” so we don't restore to a stale map later.
    /// </summary>
    public static void ClearRememberedMaps()
    {
        prevMap.Clear();
    }
}
