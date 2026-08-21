using UnityEngine;

public class HubSpawner : MonoBehaviour
{
    [SerializeField] GameObject player1Prefab;
    [SerializeField] GameObject followerPrefab;

    void Start()
    {
        HubIslandSpawnPoint[] spawnPoints =
            FindObjectsByType<HubIslandSpawnPoint>(
                FindObjectsSortMode.None
            );

        int currentIsland = 0;

        if (HubIslandSaveManager.Instance != null)
        {
            currentIsland =
                HubIslandSaveManager.Instance.CurrentIsland;
        }

        HubIslandSpawnPoint chosenSpawn = null;

        foreach (HubIslandSpawnPoint point in spawnPoints)
        {
            if (point.IslandID == currentIsland)
            {
                chosenSpawn = point;
                break;
            }
        }

        // Safety fallback.
        if (chosenSpawn == null)
        {
            Debug.LogWarning(
                $"No HubSpawnPoint found for island {currentIsland}. " +
                "Using the first available spawn point."
            );

            if (spawnPoints.Length > 0)
                chosenSpawn = spawnPoints[0];
        }

        if (chosenSpawn == null)
        {
            Debug.LogError(
                "HubSpawner could not find any HubIslandSpawnPoint!"
            );

            return;
        }

        Transform spawnPoint =
            chosenSpawn.transform;

        // Spawn Player 1.
        GameObject p1 =
            Instantiate(
                player1Prefab,
                spawnPoint.position,
                spawnPoint.rotation
            );

        // Spawn follower.
        GameObject p2 =
            Instantiate(
                followerPrefab,
                spawnPoint.position +
                Vector3.left * 1.5f,
                spawnPoint.rotation
            );

        // Hook follower target.
        HubFollower follower =
            p2.GetComponent<HubFollower>();

        if (follower != null)
            follower.SetTarget(p1.transform);

        // Hook camera target.
        HubCameraFollow cam =
            Camera.main.GetComponent<HubCameraFollow>();

        if (cam != null)
            cam.SetTarget(p1.transform);

        HubSkyColorTransition sky =
    FindFirstObjectByType<HubSkyColorTransition>();

        if (sky != null)
        {
            sky.SetCurrentIslandSkyColor(
                currentIsland
            );
        }
    }
}