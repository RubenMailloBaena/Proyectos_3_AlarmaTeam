using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerController))]
public class PlayerCrouchController : MonoBehaviour
{
    private PlayerController pController;
    
    [SerializeField] private InputActionReference crouchInput;
    [SerializeField] private Transform pitchController;

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
    }

    private void SetStandUpTargets()
    {
        if (!CanStandUp()) return;
        
        isCrouching = false;
        targetHeight = initialHeight;
        targetCenter = initialCenter;
        targetCameraPosition = initialCameraPosition;
    }

    private void PerformCrouch()
    {
        charController.center = Vector3.Lerp(charController.center, targetCenter, Time.deltaTime * crouchSpeed);
        charController.height = Mathf.Lerp(charController.height, targetHeight, Time.deltaTime * crouchSpeed);
        pitchController.localPosition = Vector3.Lerp(pitchController.localPosition, targetCameraPosition, Time.deltaTime * crouchSpeed);
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