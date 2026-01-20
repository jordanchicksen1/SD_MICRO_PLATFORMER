using UnityEngine;

public class ButtonVisual : MonoBehaviour
{
    [SerializeField] Transform buttonTop;
    [SerializeField] float pressDepth = 0.2f;
    [SerializeField] float pressSpeed = 18f;

    Vector3 topStartLocalPos;
    bool pressed;

    void Awake()
    {
        if (buttonTop == null)
        {
            Debug.LogError("ButtonVisual: buttonTop not assigned.", this);
            enabled = false;
            return;
        }

        topStartLocalPos = buttonTop.localPosition;
    }

    void Update()
    {
        Vector3 target = pressed
            ? topStartLocalPos + Vector3.down * pressDepth
            : topStartLocalPos;

        buttonTop.localPosition = Vector3.Lerp(buttonTop.localPosition, target, Time.deltaTime * pressSpeed);
    }

    public void Press() => pressed = true;
    public void Release() => pressed = false;

    public bool IsPressed() => pressed;
}
