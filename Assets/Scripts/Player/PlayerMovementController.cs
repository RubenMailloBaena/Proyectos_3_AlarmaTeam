using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementController : MonoBehaviour
{
    //References
    [SerializeField] private InputActionReference moveInput;
    
    //Movement
    [Header("Movement Attributes")] 
    [SerializeField] private float walkSpeed = 6f;
    [SerializeField] private float leaningSpeed = 3f;

    private CharacterController charController;
    private Vector2 input;
    private bool isLeaning;
    
    private void Awake()
    {
        charController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        PlayerMovement();
    }

    private void PlayerMovement()
    {
        input = moveInput.action.ReadValue<Vector2>();
        
        Vector3 moveDir = transform.right * input.x + transform.forward * input.y;

        float finalSpeed = walkSpeed;

        if (isLeaning) 
            finalSpeed = leaningSpeed;

        charController.Move(moveDir * (finalSpeed * Time.deltaTime));
    }

    public void SetIsLeaning(bool leaning) => isLeaning = leaning;
}
