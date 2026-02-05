using UnityEngine;

public class HubSpawner : MonoBehaviour
{
    [SerializeField] GameObject player1Prefab;      // HubPlayer_P1
    [SerializeField] GameObject followerPrefab;     // HubPlayer_P2Follower
    [SerializeField] Transform spawnPoint;

    void Start()
    {
        // Spawn Player 1
        GameObject p1 = Instantiate(player1Prefab, spawnPoint.position, spawnPoint.rotation);

        // Spawn follower
        GameObject p2 = Instantiate(followerPrefab, spawnPoint.position + Vector3.left * 1.5f, spawnPoint.rotation);

        // Hook follower target
        HubFollower follower = p2.GetComponent<HubFollower>();
        if (follower != null)
            follower.SetTarget(p1.transform);

        // Hook camera target
        HubCameraFollow cam = Camera.main.GetComponent<HubCameraFollow>();
        if (cam != null)
            cam.SetTarget(p1.transform);
    }
}
