using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerController))]
public class PlayerMovementController : MonoBehaviour
{
    private PlayerController pController;
    
    [SerializeField] private InputActionReference moveInput;
    [SerializeField] private InputActionReference runInput;
    [SerializeField] private InputActionReference jumpInput;

    [Header("Camera Attributes")] 
    [SerializeField] private float cameraNormalFov = 60f;
    [SerializeField] private float cameraRunFov = 70f;
    [SerializeField] private float fovSpeed = 5f;
    private Camera playerCamera;
    
    [Header("Movement Attributes")] 
    [SerializeField] private float runSpeed = 8f;
    [SerializeField] private float walkSpeed = 6f;
    [SerializeField] private float leaningSpeed = 3f;
    [SerializeField] private float crouchSpeed = 3f;

    [Header("Jump Attributes")] 
    [SerializeField] private bool canJump;
    [SerializeField] private float jumpHeight = 3f;

    [Header("Sound Attributes")]
    [SerializeField] private float walkingSoundRange = 5f;
    [SerializeField] private float runningSoundRange = 8f;
    [SerializeField] private LayerMask enemyLayer;
    private float finalRange;
    
    [Header("Other Attributes")]
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float fallFromCornerDrag = -2f;
    [SerializeField] private float floorDrag = -10f;
    [SerializeField] private float groundCheckRadius = 0.3f;
    [SerializeField] private LayerMask groundMask;
    
    
    private CharacterController charController;
    private Vector2 input;
    private Vector3 velocity, sphereOffset = new Vector3(0f, 1f, 0f);
    
    private void Awake()
    {
        charController = GetComponent<CharacterController>();
        pController = GetComponent<PlayerController>();
        playerCamera = Camera.main;
    }

    private void Update()
    {
        PlayerMovement();
        HandleCameraFOV();
        PlayerJump();
        PlayerGravity();
        PlayerSound();
    }

    private void PlayerMovement()
    {
        if (pController.IsTeleporting || pController.IsPlayerDead) return;

        input = moveInput.action.ReadValue<Vector2>();

        pController.SetIsIdle(false);
        if (input == Vector2.zero)
        {
            pController.SetIsIdle(true);
            AudioManager.Instance.HandleStopSound("event:/Jugador/jugador_pasos_madera_correr", true);
            AudioManager.Instance.HandleStopSound("event:/Jugador/jugador_pasos_madera_andar", true);
        }
            
        
        Vector3 moveDir = transform.right * input.x + transform.forward * input.y;

        charController.Move(moveDir * (GetFinalSpeed() * Time.deltaTime));
    }

    private float GetFinalSpeed()
    {
        if (pController.IsLeaning) return leaningSpeed;
        if (pController.IsCrouching) return crouchSpeed;
        if (runInput.action.ReadValue<float>() > 0 && !pController.IsUsingVision && !pController.isDamaged)
        {
            finalRange = runningSoundRange;
            pController.SetIsRunning(true);
            return runSpeed;
        }
        
        finalRange = walkingSoundRange;
        pController.SetIsRunning(false);
        return walkSpeed;
    }

    private void HandleCameraFOV()
    {
        float targetFOV = pController.IsRunning ? cameraRunFov : cameraNormalFov;

        if (Mathf.Approximately(playerCamera.fieldOfView, targetFOV))
            return;

        playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, targetFOV, Time.deltaTime * fovSpeed);
    }

    private void PlayerJump()
    {
        if (!canJump) return;
        
        if (jumpInput.action.triggered && IsGrounded() && !pController.IsLeaning)
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
        if (Physics.CheckSphere(transform.position, groundCheckRadius, groundMask))
        {
            pController.SetGrounded(true);
            return true;
        }
        pController.SetGrounded(false);
        return false;
    }

    private void PlayerSound()
    {
        if (pController.IsCrouching || input == Vector2.zero || pController.IsPlayerDead) return;
        
        foreach (IEnemyHear enemy in pController.GetHearEnemies())
        {
            if(pController.GetDistance(enemy.GetPosition()) <= finalRange)
                enemy.HeardSound(transform.position, false);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + sphereOffset, finalRange);
    }

    private void OnEnable()
    {
        jumpInput.action.Enable();
    }

    private void OnDisable()
    {
        jumpInput.action.Disable();
    }
}
