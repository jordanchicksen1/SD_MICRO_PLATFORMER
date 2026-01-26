using UnityEngine;

public class FallSaver : MonoBehaviour
{
    public GameObject respawnPoint;

    public void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            other.transform.position = respawnPoint.transform.position;
            other.GetComponent<PlayerHealth>().TakeDamage(1, other.transform.position);
        }

    }
}
