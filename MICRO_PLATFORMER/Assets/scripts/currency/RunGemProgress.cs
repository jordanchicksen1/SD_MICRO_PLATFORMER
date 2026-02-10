using System.Collections.Generic;
using UnityEngine;

public class RunGemProgress : MonoBehaviour
{
    public static RunGemProgress Instance { get; private set; }

    // Stores keys like "Level1::Gem_01" for gems collected THIS RUN
    HashSet<string> pending = new HashSet<string>();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        // NOT DontDestroyOnLoad — per level
    }

    void OnEnable()
    {
        pending.Clear(); // new run each time level loads
    }

    public static string MakeKey(string levelId, string gemId) => $"{levelId}::{gemId}";

    public void MarkPending(string levelId, string gemId)
    {
        if (string.IsNullOrEmpty(levelId) || string.IsNullOrEmpty(gemId)) return;
        pending.Add(MakeKey(levelId, gemId));
    }

    public bool IsPending(string levelId, string gemId)
    {
        if (string.IsNullOrEmpty(levelId) || string.IsNullOrEmpty(gemId)) return false;
        return pending.Contains(MakeKey(levelId, gemId));
    }

    public IEnumerable<string> GetPendingKeys() => pending;
}
