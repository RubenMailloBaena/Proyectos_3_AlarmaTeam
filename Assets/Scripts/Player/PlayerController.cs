using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //References
    [SerializeField] private InputActionReference moveInput;
    [SerializeField] private InputActionReference mouseInput;
    [SerializeField] private Transform m_PitchController;
    
    //Movement
    [Header("Movement Attributes")] 
    [SerializeField] private float playerSpeed = 5f;
    
    private CharacterController charController;
    private Vector3 direction;
    
    //Camera
    [Header("Look Around Attributes")]
    [SerializeField] private float sensitivity;
    public float m_YawSpeed;
    public float m_PitchSpeed;
    public float m_MinPitch;
    public float m_MaxPitch;
    
    private float m_Yaw;
    private float m_Pitch;
    
    
    private void Awake()
    {
        charController = GetComponent<CharacterController>();
    }

    private void Start()
    {
        m_Yaw = transform.eulerAngles.y;
        m_Pitch = m_PitchController.eulerAngles.x;
    } 
    
    private void Update()
    {
        CameraControl();
        PlayerMovement();
    }

    private void CameraControl()
    {
        Vector2 mouseDelta = mouseInput.action.ReadValue<Vector2>(); 
        
        m_Yaw += (mouseDelta.x * sensitivity) * m_YawSpeed * Time.deltaTime;
        m_Pitch += (mouseDelta.y * sensitivity) * m_PitchSpeed * Time.deltaTime;

        m_Pitch = Mathf.Clamp(m_Pitch, m_MinPitch, m_MaxPitch);
        
        transform.rotation = Quaternion.Euler(0f, m_Yaw, 0f);
        m_PitchController.localRotation = Quaternion.Euler(-m_Pitch, 0f, 0f);
    }

    private void PlayerMovement()
    {
        Vector2 input = moveInput.action.ReadValue<Vector2>();

        float forwardAngleRadians = m_Yaw * Mathf.Deg2Rad;
        float rightAngleRadians = (m_Yaw + 90f) * Mathf.Deg2Rad;

        Vector3 forward = new Vector3(Mathf.Sin(forwardAngleRadians), 0.0f, Mathf.Cos(forwardAngleRadians));
        Vector3 right = new Vector3(Mathf.Sin(rightAngleRadians), 0.0f, Mathf.Cos(rightAngleRadians));

        Vector3 movement = (forward * input.y) + (right * input.x);
        movement.Normalize();

        movement *= playerSpeed * Time.deltaTime;

        charController.Move(movement);
    }
}
