using UnityEngine;

public class OneButtonDoor : MonoBehaviour
{
    [SerializeField] Door door;

    public void OnTriggerEnter(Collider other)
    {
        door.Toggle();
    }
}
