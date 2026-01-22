using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public bool HasKey { get; private set; }

    public void GiveKey()
    {
        HasKey = true;
    }

    public void UseKey()
    {
        HasKey = false;
    }
}
