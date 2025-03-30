using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLeanController : MonoBehaviour
{
    [SerializeField] private InputActionReference leanInput;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform pitchController;
    
    private RaycastHit hit;
    private float input;
    private PlayerMovementController playerMovementController;

    private void Awake()
    {
        playerMovementController = GetComponent<PlayerMovementController>();
    }

    private void Update()
    {
        LeanPlayer();
    }

    private void LeanPlayer()
    {
        if (!playerMovementController.IsGrounded()) return;
        
        input = leanInput.action.ReadValue<float>();
        
        Vector3 pos = new Vector3(transform.position.x, pitchController.position.y, transform.position.z);
            
        if (input < 0 && !Physics.Raycast(pos, -transform.right, out hit, 1f))
        {
            animator.ResetTrigger("Idle");
            animator.ResetTrigger("Right");
            animator.SetTrigger("Left");
            playerMovementController.SetIsLeaning(true);
        }
        else if (input > 0 && !Physics.Raycast(pos, transform.right, out hit, 1f))
        {
            animator.ResetTrigger("Idle");
            animator.ResetTrigger("Left");
            animator.SetTrigger("Right");
            playerMovementController.SetIsLeaning(true);
        }
        else
        {
            animator.ResetTrigger("Right");
            animator.ResetTrigger("Left");
            animator.SetTrigger("Idle");
            playerMovementController.SetIsLeaning(false);
        }
    }
}
