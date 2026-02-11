using UnityEngine;
using UnityEngine.VFX;

public class DoorKillBox : MonoBehaviour
{
    public VisualEffect smokePoof;
    public float destroyTime = 0.125f;
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

            Destroy(other.gameObject, destroyTime);
            Debug.Log("door entered");
        }
    }
}
