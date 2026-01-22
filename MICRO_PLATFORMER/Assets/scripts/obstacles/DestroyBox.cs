using UnityEngine;

public class DoorKillBox : MonoBehaviour
{
    void OnTriggerStay(Collider other)
    {
        // OPTION A: tag-based
        if (other.CompareTag("Door"))
        {
            Destroy(other.transform.root.gameObject);
        }
    }
}
