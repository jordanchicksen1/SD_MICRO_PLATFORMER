using UnityEngine;

public class ObjectTurner : MonoBehaviour
{
    public float rotateX;
    public float rotateY;
    public float rotateZ;
    void Update()
    {
        
        this.transform.Rotate(rotateX * Time.deltaTime, rotateY * Time.deltaTime, rotateZ * Time.deltaTime);
        Debug.Log("is it working?");
    }
}
