using TMPro;
using UnityEngine;

public class EnemyKillPopup : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;

    [Header("Motion")]
    [SerializeField] float riseSpeed = 2f;

    [Header("Animation")]
    [SerializeField] float popScale = 1.6f;
    [SerializeField] float popDuration = 0.15f;

    [Header("Lifetime")]
    [SerializeField] float lifeTime = 1f;

    float timer;
    Vector3 baseScale;
    Color startColor;

    void Awake()
    {
        baseScale = transform.localScale;

        if (text)
            startColor = text.color;
    }

    public void SetNumber(int number)
    {
        if (text)
            text.text = number.ToString();
    }

    void Update()
    {
        timer += Time.deltaTime;

        // FLOAT
        transform.position += Vector3.up * riseSpeed * Time.deltaTime;

        // POP SCALE
        if (timer < popDuration)
        {
            float t = timer / popDuration;
            float scale = Mathf.Lerp(popScale, 1f, t);
            transform.localScale = baseScale * scale;
        }

        // COLOUR SHIFT DURING ENTIRE LIFETIME
        if (text)
        {
            float t = timer / lifeTime;
            text.color = Color.Lerp(startColor, Color.white, t);
        }

        if (timer >= lifeTime)
            Destroy(gameObject);
    }
}