using System.Collections.Generic;
using UnityEngine;

public class PersistentGemProgress : MonoBehaviour
{
    public static PersistentGemProgress Instance { get; private set; }

    Dictionary<string, HashSet<string>> collected = new();

    const string PREF_KEY = "GEM_PROGRESS_V1";

    // Track always so your UI works while playtesting
    bool TrackEnabled => true;

    // Only save/load to disk in builds (so editor playmode doesn't permanently delete gems)
    bool SaveToDisk =>
#if UNITY_EDITOR
        false;
#else
        true;
#endif

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (SaveToDisk) Load();
        else collected.Clear();
    }

    string MakeKey(string levelId, string gemId) => $"{levelId}|{gemId}";

    public bool IsCollected(string levelId, string gemId)
    {
        if (!TrackEnabled) return false;

        if (string.IsNullOrEmpty(levelId) || string.IsNullOrEmpty(gemId))
            return false;

        if (!collected.TryGetValue(levelId, out var set)) return false;
        return set.Contains(gemId);
    }

    // Mark permanently collected in memory; only write to disk in builds
    public bool TryMarkCollected(string levelId, string gemId)
    {
        if (!TrackEnabled) return false;

        if (string.IsNullOrEmpty(levelId) || string.IsNullOrEmpty(gemId))
            return false;

        if (!collected.TryGetValue(levelId, out var set))
        {
            set = new HashSet<string>();
            collected[levelId] = set;
        }

        if (!set.Add(gemId))
            return false; // already collected

        if (SaveToDisk) Save();
        return true;
    }

    public int GetCollectedCount(string levelId)
    {
        if (string.IsNullOrEmpty(levelId)) return 0;
        if (!collected.TryGetValue(levelId, out var set)) return 0;
        return set.Count;
    }

    // Commit all pending gems from this run for a specific levelId
    public void CommitPendingFromRun(string levelId)
    {
        if (!TrackEnabled) return;
        if (string.IsNullOrEmpty(levelId)) return;
        if (RunGemProgress.Instance == null) return;

        bool changed = false;

        foreach (var k in RunGemProgress.Instance.GetPendingKeys())
        {
            // "Level1::Gem_01"
            var parts = k.Split(new string[] { "::" }, System.StringSplitOptions.None);
            if (parts.Length != 2) continue;

            string lvl = parts[0];
            string gem = parts[1];

            if (lvl != levelId) continue;

            if (!collected.TryGetValue(lvl, out var set))
            {
                set = new HashSet<string>();
                collected[lvl] = set;
            }

            if (set.Add(gem))
                changed = true;
        }

        if (changed && SaveToDisk) Save();
    }

    [System.Serializable]
    class SaveData { public List<string> keys = new(); }

    void Save()
    {
        var data = new SaveData();

        foreach (var kv in collected)
        {
            foreach (var gemId in kv.Value)
                data.keys.Add(MakeKey(kv.Key, gemId));
        }

        PlayerPrefs.SetString(PREF_KEY, JsonUtility.ToJson(data));
        PlayerPrefs.Save();
    }

    void Load()
    {
        collected.Clear();

        if (!PlayerPrefs.HasKey(PREF_KEY)) return;

        string json = PlayerPrefs.GetString(PREF_KEY);
        if (string.IsNullOrEmpty(json)) return;

        SaveData data = JsonUtility.FromJson<SaveData>(json);
        if (data?.keys == null) return;

        foreach (string k in data.keys)
        {
            int idx = k.IndexOf('|');
            if (idx <= 0 || idx >= k.Length - 1) continue;

            string levelId = k.Substring(0, idx);
            string gemId = k.Substring(idx + 1);

            if (!collected.TryGetValue(levelId, out var set))
            {
                set = new HashSet<string>();
                collected[levelId] = set;
            }

            set.Add(gemId);
        }
    }
}
