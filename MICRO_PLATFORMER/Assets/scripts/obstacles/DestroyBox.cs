using System.Collections;
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
            Destroy(other.gameObject, destroyTime);
            Debug.Log("door entered");
            StartCoroutine(DestroyDoor());
        }
    }

    public IEnumerator DestroyDoor()
    {
        yield return new WaitForSeconds(0f);
        smokePoof.Stop();      // Ensure it's reset
        smokePoof.Play();      // Play once
        yield return new WaitForSeconds(1.3f);
        Destroy(gameObject);
    }
}
