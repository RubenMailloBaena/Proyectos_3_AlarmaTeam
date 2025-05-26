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
    
    [Header("Sound Wave Effect")]
    [SerializeField] private Transform soundEffectSphere;
    [SerializeField] private Transform soundEffectSphereBigOne;
    [SerializeField] private float pulseDuration = 0.5f;
    [SerializeField] private float pulseScale = 14f;
    private Coroutine waveEffectCoroutine;
    private float finalRange;
    private bool stoppedRunning = true;
    
    [Header("Other Attributes")]
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float fallFromCornerDrag = -2f;
    [SerializeField] private float floorDrag = -10f;
    [SerializeField] private float groundCheckRadius = 0.3f;
    [SerializeField] private LayerMask groundMask;

    private CharacterController charController;
    private Vector2 input;
    private Vector3 velocity, sphereOffset = new Vector3(0f, 1f, 0f);

    [Header("Fall Sound Attributes")]
    [SerializeField] private float minAirTime = 0.1f;
    private float airTime = 0f;
    private bool fallSoundPlaying;

    private void Awake()
    {
        charController = GetComponent<CharacterController>();
        pController = GetComponent<PlayerController>();
        playerCamera = Camera.main;
        fallSoundPlaying = false; 
    }

    private void Update()
    {
        FallJumpSound();
        PlayerMovement();
        HandleCameraFOV();
        PlayerJump();
        PlayerGravity();
        PlayerSound();
    }
    public void ResetFallSound()
    {
        airTime = 0f;

        if (fallSoundPlaying)
        {
            AudioManager.Instance.HandleStopSound("event:/Test/TestLoop", true);
            fallSoundPlaying = false;
        }
    }

    private void FallJumpSound()
    {
        bool isGrounded = IsGrounded();

        if (!isGrounded)
        {
            airTime += Time.deltaTime;

            if (!fallSoundPlaying && airTime > minAirTime && !pController.IsTeleporting && !pController.IsVaulting)
            {
                AudioManager.Instance.HandlePlaySound3D("event:/Test/TestLoop", transform.position);
                fallSoundPlaying = true;
            }
        }
        else
        {
            if (fallSoundPlaying)
            {
                AudioManager.Instance.HandleStopSound("event:/Test/TestLoop", true);
                fallSoundPlaying = false;
            }

            airTime = 0f;
        }
    }



    private void PlayerMovement()
    {
        if (pController.IsTeleporting)
        {
            stoppedRunning = true;
            return;
        }
        
        if (pController.IsPlayerDead || pController.isBackstabing) return;

        input = moveInput.action.ReadValue<Vector2>();

        pController.SetIsIdle(false);
        if (input == Vector2.zero)
        {
            pController.SetIsIdle(true);
        }
            
        
        Vector3 moveDir = transform.right * input.x + transform.forward * input.y;

        charController.Move(moveDir * (GetFinalSpeed() * Time.deltaTime));
    }

    private float GetFinalSpeed()
    {
        if (pController.IsLeaning)
        {
            if (!stoppedRunning)
                stoppedRunning = true;
            return leaningSpeed;
        }

        if (pController.IsCrouching)
        {
            if (!stoppedRunning)
                stoppedRunning = true;
            return crouchSpeed;
        }
        if (runInput.action.ReadValue<float>() > 0 && !pController.IsUsingVision && !pController.isDamaged && !pController.IsIdle && !pController.IsLeaning)
        {
            finalRange = runningSoundRange;
            pController.SetIsRunning(true);
            if (waveEffectCoroutine == null)
            {
                stoppedRunning = false;
                waveEffectCoroutine = StartCoroutine(PulseWave());
                StartCoroutine(PulseWaveBigOne());
            }
            return runSpeed;
        }
        
        finalRange = walkingSoundRange;
        stoppedRunning = true;
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
        pController.onStopFallSound += ResetFallSound;
    }

    private void OnDisable()
    {
        jumpInput.action.Disable();
        pController.onStopFallSound -= ResetFallSound;
    }
    
    
    //SOUND EFFECT ANIMATION
    private IEnumerator PulseWave()
    {
        while (!stoppedRunning)
        {
            float timer = 0f;
            while (timer < pulseDuration)
            {
                float t = timer / pulseDuration;
                float scale = Mathf.Lerp(0f, pulseScale, t);
                soundEffectSphere.localScale = Vector3.one * scale;
                timer += Time.deltaTime;
                yield return null;

                if (stoppedRunning) break;
            }
        }
        soundEffectSphere.localScale = Vector3.zero;
        waveEffectCoroutine = null;
    }
    
    private IEnumerator PulseWaveBigOne()
    {
        float expandTimer = 0f;
        Vector3 startScale = Vector3.zero;
        Vector3 targetScale = Vector3.one * pulseScale;

        while (expandTimer < pulseDuration)
        {
            float t = expandTimer / (pulseDuration / 2);
            soundEffectSphereBigOne.localScale = Vector3.Lerp(startScale, targetScale, t);
            expandTimer += Time.deltaTime;
            yield return null;
        }

        soundEffectSphereBigOne.localScale = targetScale;

        while (!stoppedRunning)
        {
            yield return null;
        }

        float shrinkTimer = 0f;
        Vector3 currentScale = soundEffectSphereBigOne.localScale;

        while (shrinkTimer < pulseDuration)
        {
            float t = shrinkTimer / (pulseDuration / 2);
            soundEffectSphereBigOne.localScale = Vector3.Lerp(currentScale, Vector3.zero, t);
            shrinkTimer += Time.deltaTime;
            yield return null;
        }

        soundEffectSphereBigOne.localScale = Vector3.zero;
        waveEffectCoroutine = null;
    }

}
