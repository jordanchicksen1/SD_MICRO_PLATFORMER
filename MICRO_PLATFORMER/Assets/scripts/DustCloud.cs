using UnityEngine;

public class DustCloud : MonoBehaviour
{
    [Header("Lifetime")]
    [SerializeField] float lifetime = 0.45f;

    [Header("Movement")]
    [SerializeField] float floatSpeed = 2.2f;
    [SerializeField] float growSpeed = 12f;

    [Header("Rotation")]
    [SerializeField] float rotationSpeedMin = 220f;
    [SerializeField] float rotationSpeedMax = 500f;

    [Header("Scale")]
    [SerializeField] float minScale = 0.10f;
    [SerializeField] float maxScale = 0.15f;

    [Header("Spawn Offset")]
    [SerializeField] float spawnOffset = 0.05f;

    float rotationSpeed;
    float timer;

    Vector3 driftDirection;
    Vector3 targetScale;

    public void Spawn(Vector3 position)
    {
        // Slight random spawn offset
        transform.position = position + new Vector3(
            Random.Range(-spawnOffset, spawnOffset),
            0f,
            Random.Range(-spawnOffset, spawnOffset));

        timer = 0f;

        float randomScale = Random.Range(minScale, maxScale);
        targetScale = Vector3.one * randomScale;

        // Start tiny for a nice pop
        transform.localScale = Vector3.zero;

        // Random starting rotation
        transform.rotation = Quaternion.Euler(
            0f,
            Random.Range(0f, 360f),
            0f);

        // Random spin speed and direction
        rotationSpeed = Random.Range(rotationSpeedMin, rotationSpeedMax);

        if (Random.value > 0.5f)
            rotationSpeed *= -1f;

        // Random drift direction
        driftDirection = new Vector3(
            Random.Range(-0.5f, 0.5f),
            1f,
            Random.Range(-0.5f, 0.5f)).normalized;

        gameObject.SetActive(true);
    }

    void Update()
    {
        timer += Time.deltaTime;

        float lifePercent = timer / lifetime;

        // Quick pop-in
        if (lifePercent < 0.2f)
        {
            transform.localScale = Vector3.MoveTowards(
                transform.localScale,
                targetScale,
                growSpeed * 3f * Time.deltaTime);
        }
        // Hold size
        else if (lifePercent < 0.6f)
        {
            transform.localScale = targetScale;
        }
        // Shrink away
        else
        {
            float shrink = Mathf.InverseLerp(0.6f, 1f, lifePercent);

            transform.localScale = Vector3.Lerp(
                targetScale,
                Vector3.zero,
                shrink);
        }

        // Float
        transform.position += driftDirection * floatSpeed * Time.deltaTime;

        // Spin
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);

        if (timer >= lifetime)
        {
            gameObject.SetActive(false);
        }
    }
}