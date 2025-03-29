using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.Text;


public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private InputActionReference moveInput;
    
    [Header("Movement Attributes")] 
    [SerializeField] private float playerSpeed = 5f;

    private CharacterController charController;
    private Vector3 direction;
    
    private void Start()
    {
        charController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        ProcessInput();
        Move();
    }

    private void ProcessInput()
    {
        Vector2 input = moveInput.action.ReadValue<Vector2>();
        direction = new Vector3(input.x, 0f, input.y).normalized;
    }

    private void Move()
    {
        charController.Move(direction * (playerSpeed * Time.deltaTime));
    }
}
