using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    [SerializeField] Door door;

    int count;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            count++;
            door.Toggle();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            count--;
            door.Toggle();
        }
    }
}
