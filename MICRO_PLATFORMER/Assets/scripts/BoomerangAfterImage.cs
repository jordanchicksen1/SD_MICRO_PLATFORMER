using UnityEngine;

public class BoomerangAfterImage : MonoBehaviour
{
    [SerializeField] float lifetime = 0.15f;
    [SerializeField, Range(0f, 1f)] float startOpacity = 0.70f;

    private float remainingLifetime;

    private Renderer rend;
    private Material mat;
    private Color startColor;

    void Awake()
    {
        rend = GetComponentInChildren<Renderer>();

        // Creates a unique material instance so each afterimage fades independently
        mat = rend.material;

        startColor = mat.color;
        startColor.a = startOpacity;
        mat.color = startColor;

        remainingLifetime = lifetime;
    }

    void Update()
    {
        transform.Rotate(0f, 1080f * Time.deltaTime, 0f);
        remainingLifetime -= Time.deltaTime;

        float alpha = Mathf.Clamp01(remainingLifetime / lifetime);

        Color c = startColor;
        c.a = startOpacity * alpha;

        mat.color = c;

        if (remainingLifetime <= 0f)
        {
            Destroy(gameObject);
        }
    }
}