using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerController))]
public class PlayerLeanController : MonoBehaviour
{
    private PlayerController pController;
    
    [SerializeField] private InputActionReference leanInput;
    [SerializeField] private Transform pitchController;
    [SerializeField] private Transform leanParent;

    [Header("Lean Attributes")]
    [SerializeField] private float moveAmount = 0.45f;
    [SerializeField] private float leanAngle = 20f;
    [SerializeField] private float leanSpeed = 5f;
    [SerializeField] private LayerMask allLayer;

    private float input;
    private Vector3 originalPosition, targetPosition;
    private Quaternion originalRotation, targetRotation;

    private void Awake()
    {
        pController = GetComponent<PlayerController>();
    }

    private void Start()
    {
        originalPosition = leanParent.localPosition;
        originalRotation = leanParent.localRotation;
    }

    private void Update()
    {
        SetLeanTarget();
        LeanPlayer();
    }

    private void SetLeanTarget()
    {
        if (!pController.IsGrounded || pController.IsPlayerDead) return;

        targetPosition = originalPosition;
        targetRotation = originalRotation;
        pController.SetLeaning(false);

        if (pController.IsVaulting) return; //SI VAULTEAMOS QUITAMOS EL LEAN
        
        input = leanInput.action.ReadValue<float>();
        
        //IDLE

        if (input < 0 && !Physics.Raycast(pitchController.position, -transform.right, 1f, allLayer)) //LEFT
        {
            targetPosition += Vector3.left * moveAmount; 
            targetRotation *= Quaternion.Euler(0, 0, leanAngle); 
            pController.SetLeaning(true);
        }
        else if (input > 0  && !Physics.Raycast(pitchController.position, transform.right, 1f, allLayer)) //RIGHT
        {
            targetPosition += Vector3.right * moveAmount; 
            targetRotation *= Quaternion.Euler(0, 0, -leanAngle); 
            pController.SetLeaning(true);
        }
    }

    private void LeanPlayer()
    {
        leanParent.localPosition = Vector3.Lerp(leanParent.localPosition, targetPosition, Time.deltaTime * leanSpeed);
        leanParent.localRotation = Quaternion.Slerp(leanParent.localRotation, targetRotation, Time.deltaTime * leanSpeed);
    }
}