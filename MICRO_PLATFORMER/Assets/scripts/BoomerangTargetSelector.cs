using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BoomerangTargetSelector : MonoBehaviour
{

    List<BoomerangTarget> selectedTargets = new List<BoomerangTarget>();
    BoomerangTarget currentTarget;
    [SerializeField] int maxTargets = 3;
    [SerializeField] TMP_Text targetCounterText;
    Camera playerCamera;
    [SerializeField] float reticleSpeed = 1000f;
    [SerializeField] float maxRadius = 300f;
    Vector2 reticlePosition;
    PlayerController3D playerController;
    bool isAiming;
    RectTransform reticle;
    PlayerInput playerInput;
    Vector3 reticleOriginalScale;
    Coroutine reticleAnimation;
    AudioSource audioSource;
    [SerializeField] AudioClip targetSelectSFX;

    void Awake()
    {
        playerCamera = Camera.main;
        audioSource = GetComponent<AudioSource>();
        playerController = GetComponent<PlayerController3D>();
        playerInput = GetComponent<PlayerInput>();
        GameObject reticleObject;


        if (playerInput.playerIndex == 0)
        {
            reticleObject = GameObject.Find("P1Reticle");

        }
        else
        {
            reticleObject = GameObject.Find("P2Reticle");
        }

        Debug.Log("Reticle Object = " + reticleObject);

        if (reticleObject == null)
        {
            Debug.LogError("Couldn't find reticle!");
            return;
        }

        reticle = reticleObject.GetComponentInChildren<RectTransform>();
        reticleOriginalScale = reticle.localScale;
        targetCounterText = reticleObject.transform.Find("TargetCounter").GetComponent<TMP_Text>();
        UpdateCounter();
        reticle.gameObject.SetActive(false);
        Debug.Log(gameObject.name + " is using " + reticle.name);
    }
    public void BeginAim()
    {
        isAiming = true;
        selectedTargets.Clear();
        UpdateCounter();
        currentTarget = null;
        reticle.gameObject.SetActive(true);

        if (reticleAnimation != null)
            StopCoroutine(reticleAnimation);

        reticleAnimation = StartCoroutine(PopInReticle());
        reticlePosition = Vector2.zero;
        reticle.anchoredPosition = reticlePosition;
    }

    void Update()
    {
        if (!isAiming)
            return;

        Vector2 input = playerController.MoveInput;
        reticlePosition += input * reticleSpeed * Time.deltaTime;
        reticlePosition = Vector2.ClampMagnitude(reticlePosition, maxRadius);
        reticle.anchoredPosition = reticlePosition;

        Vector2 reticleScreenPosition = RectTransformUtility.WorldToScreenPoint(null, reticle.position);

        Ray ray = playerCamera.ScreenPointToRay(reticleScreenPosition);
        Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red);

        currentTarget = null;

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            BoomerangTarget target = hit.collider.GetComponent<BoomerangTarget>();

            if (target != null)
            {
                currentTarget = target;
                TryAddTarget(target);
            }

            if (currentTarget != null)
            {
                Debug.DrawLine(playerCamera.transform.position, currentTarget.transform.position, Color.green);
            }
        }
    }

    public void AddCurrentTarget()
    {
        if (currentTarget == null)
            return;

        if (selectedTargets.Contains(currentTarget))
            return;

        selectedTargets.Add(currentTarget);

        Debug.Log(currentTarget.name + " selected!");
    }

    void TryAddTarget(BoomerangTarget target)
    {
        if (target == null)
            return;

        if (selectedTargets.Contains(target))
            return;

        if (selectedTargets.Count >= maxTargets)
            return;

        selectedTargets.Add(target);
        if (reticleAnimation != null)
            StopCoroutine(reticleAnimation);

        if (targetSelectSFX != null)
        {
            audioSource.PlayOneShot(targetSelectSFX);
        }
        reticleAnimation = StartCoroutine(ReticleSelectPop());
        UpdateCounter();
        target.ShowMarker(playerInput.playerIndex);
        Debug.Log(target.name + " Added!");
    }

    public void EndAim()
    {
        isAiming = false;
        UpdateCounter();
        if (reticleAnimation != null)
            StopCoroutine(reticleAnimation);

        reticleAnimation = StartCoroutine(PopOutReticle());
    }

    void UpdateCounter()
    {
        int remainingTargets = maxTargets - selectedTargets.Count;

        targetCounterText.text = remainingTargets.ToString();
    }

    System.Collections.IEnumerator PopInReticle()
    {
        float timer = 0f;
        float duration = 0.15f;

        Vector3 start = Vector3.zero;
        Vector3 overshoot = reticleOriginalScale * 1.2f;

        reticle.localScale = start;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float t = timer / duration;

            reticle.localScale =
                Vector3.Lerp(start, overshoot, t);

            yield return null;
        }

        timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float t = timer / duration;

            reticle.localScale =
                Vector3.Lerp(overshoot, reticleOriginalScale, t);

            yield return null;
        }

        reticle.localScale = reticleOriginalScale;
    }

    System.Collections.IEnumerator PopOutReticle()
    {
        float timer = 0f;
        float duration = 0.12f;

        Vector3 overshoot = reticleOriginalScale * 1.2f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float t = timer / duration;

            reticle.localScale =
                Vector3.Lerp(reticleOriginalScale, overshoot, t);

            yield return null;
        }

        timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float t = timer / duration;

            reticle.localScale =
                Vector3.Lerp(overshoot, Vector3.zero, t);

            yield return null;
        }

        reticle.localScale = reticleOriginalScale;
        reticle.gameObject.SetActive(false);
    }

    IEnumerator ReticleSelectPop()
    {
        float timer = 0f;
        float duration = 0.08f;

        Vector3 start = reticleOriginalScale;
        Vector3 pop = reticleOriginalScale * 1.1f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float t = timer / duration;

            reticle.localScale =
                Vector3.Lerp(start, pop, t);

            yield return null;
        }

        timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float t = timer / duration;

            reticle.localScale =
                Vector3.Lerp(pop, start, t);

            yield return null;
        }

        reticle.localScale = start;
    }

    public void ResetBoomerang()
    {
        selectedTargets.Clear();

        currentTarget = null;

        isAiming = false;

        if (reticleAnimation != null)
            StopCoroutine(reticleAnimation);

        if (reticle != null)
            reticle.gameObject.SetActive(false);

        UpdateCounter();
    }

    public List<BoomerangTarget> GetSelectedTargets()
    {
        return new List<BoomerangTarget>(selectedTargets);
    }
}