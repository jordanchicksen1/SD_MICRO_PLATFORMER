using UnityEngine;

public class GroundSlamDustCloud : MonoBehaviour
{
    [Header("Lifetime")]
    [SerializeField] float lifetime = 0.9f;

    [Header("Growth")]
    [SerializeField] float growSpeed = 8f;

    [Header("Rotation")]
    [SerializeField] float rotationSpeedMin = 80f;
    [SerializeField] float rotationSpeedMax = 180f;

    [Header("Scale")]
    [SerializeField] float minScale = 0.35f;
    [SerializeField] float maxScale = 0.6f;

    [Header("Spawn Offset")]
    [SerializeField] float spawnOffset = 0.05f;

    float timer;
    float rotationSpeed;
    Vector3 targetScale;

    public void Spawn(Vector3 position, Vector3 direction)
    {
        transform.position = position +
            new Vector3(
                Random.Range(-spawnOffset, spawnOffset),
                0f,
                Random.Range(-spawnOffset, spawnOffset));

        timer = 0f;

        float randomScale = Random.Range(minScale, maxScale);
        targetScale = Vector3.one * randomScale;

        transform.localScale = Vector3.zero;

        transform.rotation = Quaternion.Euler(
            0f,
            Random.Range(0f, 360f),
            0f);

        rotationSpeed = Random.Range(rotationSpeedMin, rotationSpeedMax);

        if (Random.value > 0.5f)
            rotationSpeed *= -1f;

        gameObject.SetActive(true);
    }

    void Update()
    {
        timer += Time.deltaTime;

        float t = timer / lifetime;

        if (t < 0.2f)
        {
            transform.localScale = Vector3.MoveTowards(
                transform.localScale,
                targetScale,
                growSpeed * Time.deltaTime);
        }
        else if (t > 0.7f)
        {
            transform.localScale = Vector3.Lerp(
                targetScale,
                Vector3.zero,
                Mathf.InverseLerp(0.7f, 1f, t));
        }
        else
        {
            transform.localScale = targetScale;
        }

        // Rotate only.
        transform.Rotate(
            Vector3.up,
            rotationSpeed * Time.deltaTime,
            Space.World);

        if (timer >= lifetime)
        {
            gameObject.SetActive(false);
        }
    }
}