using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BoomerangTargetSelector : MonoBehaviour
{

    List<BoomerangTarget> selectedTargets = new List<BoomerangTarget>();
    BoomerangTarget currentTarget;
    [SerializeField] int maxTargets = 3;
    Camera playerCamera;
    [SerializeField] float reticleSpeed = 1000f;
    [SerializeField] float maxRadius = 300f;
    Vector2 reticlePosition;
    PlayerController3D playerController;
    bool isAiming;
    RectTransform reticle;
    PlayerInput playerInput;


    void Awake()
    {
        playerCamera = Camera.main;
        
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

        reticle = reticleObject.GetComponent<RectTransform>();
        reticle.gameObject.SetActive(false);
        Debug.Log(gameObject.name + " is using " + reticle.name);
    }
    public void BeginAim()
    {
        isAiming = true;
        selectedTargets.Clear();
        currentTarget = null;
        reticle.gameObject.SetActive(true);
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
        target.ShowMarker(playerInput.playerIndex);
        Debug.Log(target.name + " Added!");
    }

    public void EndAim()
    {
        isAiming = false;
        reticle.gameObject.SetActive(false);
    }

    public List<BoomerangTarget> GetSelectedTargets()
    {
        return new List<BoomerangTarget>(selectedTargets);
    }
}