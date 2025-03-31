using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLookController : MonoBehaviour
{
    [SerializeField] private InputActionReference mouseInput;
    [SerializeField] private Transform pitchController;
    
    [Header("Look Attributes")] 
    [SerializeField] private float sensitivity_X;
    [SerializeField] private float sensitivity_Y;
    [SerializeField] private float maxTopLook = 80f;
    [SerializeField] private float maxBottomLook = -60f;

    private Vector2 input;
    private float xRotation = 0f;
    private bool cameraLocked;

    private void Awake()
    {
        GameManager.GetInstance().SetPlayerLookController(this);
    }

    private void Start()
    {
        LockCursor();
    } 

    private void Update()
    {
        CameraControl();
    }

    private void CameraControl()
    {
        if (cameraLocked) return;

        input = mouseInput.action.ReadValue<Vector2>();

        float mouseX = input.x * Time.deltaTime * sensitivity_X;
        float mouseY = input.y * Time.deltaTime * sensitivity_Y;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, maxBottomLook, maxTopLook);

        pitchController.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        cameraLocked = false;
    }
    public void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        cameraLocked = true;
    }
}
