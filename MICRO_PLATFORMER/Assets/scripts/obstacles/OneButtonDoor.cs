using UnityEngine;

public class OneButtonDoor : MonoBehaviour
{
    [SerializeField] Door door;

    public void Interact(PlayerController3D player)
    {
        door.Toggle();
    }
}
