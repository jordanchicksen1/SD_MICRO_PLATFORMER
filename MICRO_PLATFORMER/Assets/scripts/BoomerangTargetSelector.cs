using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BoomerangTargetSelector : MonoBehaviour
{

    
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
    }

    public void EndAim()
    {
        isAiming = false;
        reticle.gameObject.SetActive(false);
    }
}