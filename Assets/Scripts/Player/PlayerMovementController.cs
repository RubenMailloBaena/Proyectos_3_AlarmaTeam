using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementController : MonoBehaviour
{
    [SerializeField] private InputActionReference moveInput;
    [SerializeField] private InputActionReference jumpInput;
    
    [Header("Movement Attributes")] 
    [SerializeField] private float walkSpeed = 6f;
    [SerializeField] private float leaningSpeed = 3f;
    [SerializeField] private float crouchSpeed = 3f;

    [Header("Jump Attributes")] [SerializeField]
    private float jumpHeight = 3f;
    
    [Header("Other Attributes")]
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float fallFromCornerDrag = -2f;
    [SerializeField] private float floorDrag = -10f;
    [SerializeField] private float groundCheckRadius = 0.4f;
    [SerializeField] private LayerMask groundMask;
    
    private CharacterController charController;
    private Vector2 input;
    private Vector3 velocity;
    
    private bool isLeaning;
    private bool isCrouching;
    
    private void Awake()
    {
        charController = GetComponent<CharacterController>();
        GameManager.GetInstance().SetPlayerMovement(this);
    }

    private void Update()
    {
        PlayerMovement();
        PlayerJump();
        PlayerGravity();
    }

    private void PlayerMovement()
    {
        input = moveInput.action.ReadValue<Vector2>();
        
        Vector3 moveDir = transform.right * input.x + transform.forward * input.y;

        charController.Move(moveDir * (GetFinalSpeed() * Time.deltaTime));
    }

    private float GetFinalSpeed()
    {
        if (isLeaning) return leaningSpeed;
        if (isCrouching) return crouchSpeed;
        return walkSpeed;
    }

    private void PlayerJump()
    {
        if (jumpInput.action.triggered && IsGrounded() && !isLeaning)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
    }

    private void PlayerGravity()
    {
        //Si estamos en una rampa o en el suelo, estamos muy pegados al suelo, asi en las rampas no rebotamos.
        //Si dejamos de estar en el suelo sin saltar, cayendo de una rampa por ejemplo, caemos normal, sim caer muy rapido
        if (IsGrounded() && velocity.y < 0)
            velocity.y = Physics.Raycast(transform.position, Vector3.down, groundCheckRadius) 
                ? floorDrag : fallFromCornerDrag;

        velocity.y += gravity * Time.deltaTime;
        charController.Move(velocity * Time.deltaTime);
    }

    public bool IsGrounded()
    {
        return Physics.CheckSphere(transform.position, groundCheckRadius, groundMask);
    }

    public void SetIsLeaning(bool leaning) => isLeaning = leaning;
    public void SetIsCrouching(bool crouching) => isCrouching = crouching;

    private void OnEnable()
    {
        jumpInput.action.Enable();
    }

    private void OnDisable()
    {
        jumpInput.action.Disable();
    }
}
