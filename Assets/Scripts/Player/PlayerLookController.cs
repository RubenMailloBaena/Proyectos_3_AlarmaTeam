using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerController))]
public class PlayerLookController : MonoBehaviour
{
    private PlayerController pController;
    
    [SerializeField] private InputActionReference mouseInput;
    [SerializeField] private Transform pitchController;
    
    [Header("Look Attributes")] 
    [SerializeField] private float smoothCamVelocity = 5f;
    [SerializeField] private float sensitivity_X;
    [SerializeField] private float sensitivity_Y;
    [SerializeField] private float maxTopLook = 80f;
    [SerializeField] private float maxBottomLook = -60f;

    private Vector2 input;
    private float xRotation = 0f;
    private bool cameraLocked;

    private void Awake()
    {
        pController = GetComponent<PlayerController>();
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
        if (pController.isBackstabing)
        {
            SmoothLookToCenter();
            return;
        }
        
        if (cameraLocked || pController.IsVaulting || pController.IsTeleporting || pController.IsPlayerDead) return;

        input = mouseInput.action.ReadValue<Vector2>();

        float mouseX = input.x * Time.deltaTime * sensitivity_X;
        float mouseY = input.y * Time.deltaTime * sensitivity_Y;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, maxBottomLook, maxTopLook);

        pitchController.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }
    
    private void SmoothLookToCenter()
    {
        xRotation = Mathf.Lerp(xRotation, 0f, Time.deltaTime * smoothCamVelocity);
        pitchController.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        Quaternion targetRotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * smoothCamVelocity);
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

    public void ChangeCameraLock(bool lockCam)
    {
        if(lockCam) LockCursor();
        else UnlockCursor();
    }

    public void RestartRotation()
    {
        xRotation = 0.0f;
        pitchController.localRotation = Quaternion.identity;
    }

    private void OnEnable()
    {
        pController.OnCameraLockChange += ChangeCameraLock;
        pController.onRestart += RestartRotation;
        pController.onRestartFromCheckpoint += RestartRotation;
    }

    private void OnDisable()
    {
        pController.OnCameraLockChange -= ChangeCameraLock;
        pController.onRestart -= RestartRotation;
        pController.onRestartFromCheckpoint -= RestartRotation;
    }
}
