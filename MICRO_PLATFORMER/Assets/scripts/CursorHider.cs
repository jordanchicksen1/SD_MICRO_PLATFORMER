using UnityEngine;

public class CursorHider : MonoBehaviour
{
   
    void Awake()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
