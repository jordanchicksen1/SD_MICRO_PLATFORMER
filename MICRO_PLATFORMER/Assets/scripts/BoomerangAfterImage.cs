using UnityEngine;

public class BoomerangAfterImage : MonoBehaviour
{
    [SerializeField] float lifetime = 0.15f;

    Renderer rend;
    Material mat;
    Color startColour;

    void Awake()
    {

        rend = GetComponentInChildren<Renderer>();
        mat = rend.material;
        startColour = mat.color;
    }

    void Update()
    {
        transform.Rotate(0f, 1080f * Time.deltaTime, 0f);
        lifetime -= Time.deltaTime;

        Color c = startColour;
        c.a = Mathf.Clamp01(lifetime / 0.15f);

        mat.color = c;

        if (lifetime <= 0f)
            Destroy(gameObject);
    }
}