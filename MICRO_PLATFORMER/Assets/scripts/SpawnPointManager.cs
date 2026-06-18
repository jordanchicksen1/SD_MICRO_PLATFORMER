using UnityEngine;

public class SpawnPointManager : MonoBehaviour
{
    public static SpawnPointManager Instance;

    [SerializeField] Transform player1Spawn;
    [SerializeField] Transform player2Spawn;

    void Awake()
    {
        Instance = this;
    }

    public Transform GetSpawnPoint(int playerIndex)
    {
        if (playerIndex == 0)
            return player1Spawn;

        return player2Spawn;
    }
}