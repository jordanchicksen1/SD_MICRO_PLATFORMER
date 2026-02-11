using UnityEngine;
using UnityEngine.VFX;

public class DoorKillBox : MonoBehaviour
{
    public VisualEffect smokePoof;

    public void Start()
    {
        smokePoof.Stop();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Door"))
        {
            smokePoof.Stop();      // Ensure it's reset
            smokePoof.Play();      // Play once

            Destroy(other.gameObject, 0.1f);
            Debug.Log("door entered");
        }
    }
}
