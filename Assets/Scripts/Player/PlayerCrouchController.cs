using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCrouchController : MonoBehaviour
{
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
    private Vector3 targetCenter;
    private Vector3 targetCameraPosition;
    
    private PlayerMovementController playerMovement;
    private CharacterController charController;
    
    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovementController>();
        charController = GetComponent<CharacterController>();
    }

    private void Start()
    {
        initialHeight = charController.height;
        initialCenter = charController.center;
        initialCameraPosition = pitchController.localPosition;

        targetHeight = initialHeight;
        targetCenter = initialCenter;
        targetCameraPosition = initialCameraPosition;
    }

    private void Update()
    {
        CrouchInput();
        PerformCrouch();
    }

    private void CrouchInput()
    {
        if (crouchInput.action.triggered)
        {
            if (!isCrouching)
            {
                isCrouching = true;
                SetCrouchTargets();
            }
            else if (CanStandUp())
            {
                isCrouching = false;
                SetStandUpTargets();
            }
        }

        playerMovement.SetIsCrouching(isCrouching);
    }

    private void SetCrouchTargets()
    {
        targetHeight = charControllerHeight;
        targetCenter = new Vector3(0, charControllerCrouchCenter, 0);
        targetCameraPosition = initialCameraPosition - new Vector3(0, cameraCrouchAmount, 0);
    }

    private void SetStandUpTargets()
    {
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
        crouchInput.action.Enable();
    }

    private void OnDisable()
    {
        crouchInput.action.Disable();
    }
}