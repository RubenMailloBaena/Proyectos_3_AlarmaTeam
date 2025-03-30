using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLeanController : MonoBehaviour
{
    [SerializeField] private InputActionReference leanInput;
    [SerializeField] private Transform pitchController;
    [SerializeField] private Transform leanParent;

    [Header("Lean Attributes")]
    [SerializeField] private float moveAmount = 0.45f;
    [SerializeField] private float leanAngle = 20f;
    [SerializeField] private float leanSpeed = 5f;

    private float input;
    private PlayerMovementController playerMovementController;
    private Vector3 originalPosition, targetPosition;
    private Quaternion originalRotation, targetRotation;

    private void Awake()
    {
        playerMovementController = GetComponent<PlayerMovementController>();
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
        if (!playerMovementController.IsGrounded()) return;

        input = leanInput.action.ReadValue<float>();
        
        //IDLE
        targetPosition = originalPosition;
        targetRotation = originalRotation;
        playerMovementController.SetIsLeaning(false);

        Vector3 pos = new Vector3(transform.position.x, pitchController.position.y, transform.position.z);
        
        if (input < 0 && !Physics.Raycast(pos, -transform.right, 1f)) //LEFT
        {
            targetPosition += Vector3.left * moveAmount; 
            targetRotation *= Quaternion.Euler(0, 0, leanAngle); 
            playerMovementController.SetIsLeaning(true);
        }
        else if (input > 0  && !Physics.Raycast(pos, transform.right, 1f)) //RIGHT
        {
            targetPosition += Vector3.right * moveAmount; 
            targetRotation *= Quaternion.Euler(0, 0, -leanAngle); 
            playerMovementController.SetIsLeaning(true);
        }
    }
    
    private void LeanPlayer()
    {
        leanParent.localPosition = Vector3.Lerp(leanParent.localPosition, targetPosition, Time.deltaTime * leanSpeed);
        leanParent.localRotation = Quaternion.Slerp(leanParent.localRotation, targetRotation, Time.deltaTime * leanSpeed);
    }
}