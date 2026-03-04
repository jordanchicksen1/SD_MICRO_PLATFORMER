using UnityEngine;

public class StarOrbit : MonoBehaviour
{
    public float rotateSpeed = 120f;

    void Update()
    {
        transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime, Space.World);
    }
}