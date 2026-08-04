using System.Collections.Generic;
using UnityEngine;

public class GroundSlamDustPool : MonoBehaviour
{
    public static GroundSlamDustPool Instance;

    [SerializeField] GameObject dustPrefab;
    [SerializeField] int poolSize = 250;

    List<GroundSlamDustCloud> pool = new();

    void Awake()
    {
        Instance = this;

        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(dustPrefab, transform);
            obj.SetActive(false);
            pool.Add(obj.GetComponent<GroundSlamDustCloud>());
        }
    }

    public void Spawn(Vector3 position, Vector3 direction)
    {
        foreach (GroundSlamDustCloud cloud in pool)
        {
            if (!cloud.gameObject.activeSelf)
            {
                cloud.Spawn(position, direction);
                return;
            }
        }
    }
}