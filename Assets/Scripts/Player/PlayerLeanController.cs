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
    private RaycastHit hit;
    private float input;
    private PlayerMovementController _playerMovementController;

    private void Awake()
    {
        _playerMovementController = GetComponent<PlayerMovementController>();
    }

    private void Update()
    {
        LeanPlayer();
    }

    private void LeanPlayer()
    {
        input = leanInput.action.ReadValue<float>();

        if (input < 0 && !Physics.Raycast(transform.position, -transform.right, out hit, 1f))
        {
            animator.ResetTrigger("Idle");
            animator.ResetTrigger("Right");
            animator.SetTrigger("Left");
            _playerMovementController.SetIsLeaning(true);
        }
        else if (input > 0 && !Physics.Raycast(transform.position, transform.right, out hit, 1f))
        {
            animator.ResetTrigger("Idle");
            animator.ResetTrigger("Left");
            animator.SetTrigger("Right");
            _playerMovementController.SetIsLeaning(true);
        }
        else
        {
            animator.ResetTrigger("Right");
            animator.ResetTrigger("Left");
            animator.SetTrigger("Idle");
            _playerMovementController.SetIsLeaning(false);
        }
    }
}
