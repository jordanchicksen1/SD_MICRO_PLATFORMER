using System.Collections;
using UnityEngine;

public class LevelCompletionTorus : MonoBehaviour
{
    [Header("Level Info")]
    [SerializeField] string levelId = "Level1";
    [SerializeField] int totalGemsInLevel = 3;

    [Header("Visual")]
    [SerializeField] Renderer torusRenderer;
    [SerializeField] Material incompleteMaterial;
    [SerializeField] Material completeMaterial;

    void Start()
    {
        StartCoroutine(WaitForSaveSystem());
    }

    IEnumerator WaitForSaveSystem()
    {
        // Wait until the save system exists
        while (PersistentGemProgress.Instance == null)
            yield return null;

        UpdateVisual();
    }

    public void UpdateVisual()
    {
        if (!torusRenderer) return;

        int collected = PersistentGemProgress.Instance.GetCollectedCount(levelId);
        bool complete = collected >= totalGemsInLevel;

        torusRenderer.material = complete ? completeMaterial : incompleteMaterial;

        Debug.Log($"[LevelCompletionTorus] {levelId} collected {collected}/{totalGemsInLevel} complete={complete}");
    }
}