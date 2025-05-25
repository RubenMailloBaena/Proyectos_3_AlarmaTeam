using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerController))]
public class PlayerCrouchController : MonoBehaviour
{
    private PlayerController pController;
    private PlayerHUDController hudController;
    
    [SerializeField] private InputActionReference crouchInput;
    [SerializeField] private Transform pitchController;

    [Header("Crouch UI alpha values")] 
    [SerializeField] private float maxAlpha = 1f;

    [Header("Crouch Attributes")] 
    [SerializeField] private float crouchSpeed = 5f;
    [SerializeField] private float cameraCrouchAmount = 0.5f;
    [SerializeField] private float charControllerHeight = 0.5f;
    [SerializeField] private float charControllerCrouchCenter = 0.5f;
    [SerializeField] private float checkIfCanStandUpRayDistance = 1.5f;
    [SerializeField] private LayerMask groundMask;

    private float initialHeight;
    private Vector3 initialCenter;
    private Vector3 initialCameraPosition;
    
    private bool isCrouching;
    private float targetHeight;
    private float targetAlpha;
    private float animationCrouchSpeed;
    private Vector3 targetCenter;
    private Vector3 targetCameraPosition;
    
    private CharacterController charController;
    
    private void Awake()
    {
        charController = GetComponent<CharacterController>();
        pController = GetComponent<PlayerController>();
    }
    
    private void Start()
    {
        hudController = GameManager.GetInstance().GetPlayerHUD();
        
        initialHeight = charController.height;
        initialCenter = charController.center;
        initialCameraPosition = pitchController.localPosition;

        targetHeight = initialHeight;
        targetCenter = initialCenter;
        targetCameraPosition = initialCameraPosition;
        animationCrouchSpeed = crouchSpeed * 2;
    }

    private void Update()
    {
        CrouchInput();
        PerformCrouch();
    }

    private void CrouchInput()
    {
        if (pController.IsPlayerDead) return;

        if (pController.isBackstabing)
        {
            crouchSpeed = animationCrouchSpeed;
            SetStandUpTargets();
            return;
        }
        else
        {
            crouchSpeed = animationCrouchSpeed / 2;
        }

        if (crouchInput.action.triggered && !pController.IsGamePaused)
        {
            if (!isCrouching)
                SetCrouchTargets();
            else
                SetStandUpTargets();
        }

        pController.SetCrouching(isCrouching);
    }

    private void SetCrouchTargets()
    {
        isCrouching = true;
        targetHeight = charControllerHeight;
        targetCenter = new Vector3(0, charControllerCrouchCenter, 0);
        targetCameraPosition = initialCameraPosition - new Vector3(0, cameraCrouchAmount, 0);
        targetAlpha = maxAlpha;

        //AudioManager.Instance.HandlePlay3DOneShot("event:/Test/TestOneShot", transform.position);
    }

    private void SetStandUpTargets()
    {
        if (!CanStandUp()) return;
        
        isCrouching = false;
        targetHeight = initialHeight;
        targetCenter = initialCenter;
        targetCameraPosition = initialCameraPosition;
        targetAlpha = 0.0f;

        //AudioManager.Instance.HandlePlay3DOneShot("event:/Test/TestOneShot", transform.position);
    }

    private void PerformCrouch()
    {
        charController.center = Vector3.Lerp(charController.center, targetCenter, Time.deltaTime * crouchSpeed);
        charController.height = Mathf.Lerp(charController.height, targetHeight, Time.deltaTime * crouchSpeed);
        pitchController.localPosition = Vector3.Lerp(pitchController.localPosition, targetCameraPosition, Time.deltaTime * crouchSpeed);
        hudController.SetCrouchVisualColor(Mathf.Lerp(hudController.GetCrouchAlphaValue(),targetAlpha, Time.deltaTime * crouchSpeed));
    }

    private bool CanStandUp()
    {
        return !Physics.Raycast(transform.position, Vector3.up, checkIfCanStandUpRayDistance, groundMask);
    }

    private void OnEnable()
    {
        pController.OnVaultCrouched += SetStandUpTargets;
        crouchInput.action.Enable();
    }

    private void OnDisable()
    {
        pController.OnVaultCrouched -= SetStandUpTargets;
        crouchInput.action.Disable();
    }
}