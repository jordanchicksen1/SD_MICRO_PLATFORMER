using System.Collections.Generic;
using UnityEngine;

public class DustPool : MonoBehaviour
{
    public static DustPool Instance;

    [SerializeField] GameObject dustPrefab;
    [SerializeField] int poolSize = 20;

    List<DustCloud> pool = new();

    void Awake()
    {
        Instance = this;

        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(dustPrefab, transform);

            obj.SetActive(false);

            pool.Add(obj.GetComponent<DustCloud>());
        }
    }

    public void Spawn(Vector3 position)
    {
        foreach (DustCloud cloud in pool)
        {
            if (!cloud.gameObject.activeSelf)
            {
                cloud.Spawn(position);
                return;
            }
        }
    }
}