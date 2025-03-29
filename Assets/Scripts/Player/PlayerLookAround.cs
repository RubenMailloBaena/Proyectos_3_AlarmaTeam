using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLookAround : MonoBehaviour
{
    [SerializeField] private InputActionReference mouseInput;
    
    [Header("References")]
    [SerializeField] private Transform m_PitchController;
    
    [Header("Look Around Attributes")]
    [SerializeField]
    private float sensivity;
    private float m_Yaw;
    private float m_Pitch;
    
    public float m_YawSpeed;
    public float m_PitchSpeed;
    public float m_MinPitch;
    public float m_MaxPitch;

    private void Update()
    {
        Vector2 mouseDelta = mouseInput.action.ReadValue<Vector2>(); 
        
        m_Yaw += (mouseDelta.x * sensivity) * m_YawSpeed * Time.deltaTime;
        m_Pitch += (mouseDelta.y * sensivity) * m_PitchSpeed * Time.deltaTime;

        m_Pitch = Mathf.Clamp(m_Pitch, m_MinPitch, m_MaxPitch);
        
        transform.rotation = Quaternion.Euler(0f, m_Yaw, 0f);
        m_PitchController.localRotation = Quaternion.Euler(-m_Pitch, 0f, 0f);
    }
}
