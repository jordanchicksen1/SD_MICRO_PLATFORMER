using UnityEngine;

public class LevelCompletionTorus : MonoBehaviour
{
    [Header("Level Info")]
    [SerializeField] string levelId = "Level1";
    [SerializeField] int totalGemsInLevel = 3;

    [Header("Visual")]
    [SerializeField] Renderer torusRenderer;
    [SerializeField] Material incompleteMaterial; // red
    [SerializeField] Material completeMaterial;   // green

    void Start()
    {
        UpdateVisual();
    }

    void OnEnable()
    {
        UpdateVisual();
    }

    public void UpdateVisual()
    {
        if (!torusRenderer) return;
        if (PersistentGemProgress.Instance == null) return;

        int collected = PersistentGemProgress.Instance.GetCollectedCount(levelId);

        bool complete = collected >= totalGemsInLevel;

        torusRenderer.material = complete ? completeMaterial : incompleteMaterial;

        Debug.Log($"[LevelCompletionTorus] {levelId} collected {collected}/{totalGemsInLevel} complete={complete}");
    }
}
