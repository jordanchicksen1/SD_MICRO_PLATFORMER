using UnityEngine;

public class DoorKillBox : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("Door"))
        {
            Destroy(other.gameObject);
            Debug.Log("door entered");
        }
    }
}
