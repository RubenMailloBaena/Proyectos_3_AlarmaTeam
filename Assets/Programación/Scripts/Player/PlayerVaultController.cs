using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerController))]
public class PlayerVaultController : MonoBehaviour, IPlayerComponent
{
    private PlayerController pController;

    [SerializeField] private InputActionReference jumpInput;

    [Header("Vault Attributes")]
    [SerializeField] private bool standUpWhenVaulting;
    [SerializeField] private float vaultSpeed = 5f;
    [SerializeField] private float vaultCheckRayDistance = 1f;
    [SerializeField] private LayerMask vaultLayer;
    [SerializeField] private LayerMask groundMask;

    [Header("Descent Smoothing")]
    [SerializeField] private float raycastStartHeight = 0.3f;
    [SerializeField] private float raycastMaxDistance = 2f;
    [SerializeField] private float descendTime = 0.2f;
    
    [Header("Vault Rotation")]
    [SerializeField] private Animator cameraAnimator;

    private ICanVault currentVaultObject;

    private void Awake()
    {
        pController = GetComponent<PlayerController>();
    }

    private void Update()
    {
        CheckIfCanVault();
    }

    private void CheckIfCanVault()
    {
        if (pController.IsVaulting || pController.IsUsingVision)
        {
            pController.HideInteract(this);
            return;
        }

        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, vaultCheckRayDistance, vaultLayer))
        {
            pController.SetCanVault(true);

            if (jumpInput.action.triggered && !pController.IsGamePaused) 
            {
                if (pController.IsCrouching && standUpWhenVaulting)
                {
                    pController.TryVaultCrouched();
                    return;
                }

                if (hit.transform.TryGetComponent(out ICanVault vaultObject))
                {
                    currentVaultObject = vaultObject;
                    StartCoroutine(PerformVault(vaultObject.GetVaultEndPoint(transform.position), vaultObject.VaultOption));
                }
            }

            pController.CanInteract(jumpInput, InputType.Press, this, ActionType.Vault);
        }
        else
        {
            pController.HideInteract(this);
            pController.SetCanVault(false);
        }
    }
    
    /*private IEnumerator PerformVault(Vector3 targetPosition)
    {
        pController.SetVaulting(true);

        string soundEvent = GetVaultSoundEvent();
        AudioManager.Instance.HandlePlaySound3D(soundEvent, transform.position);

        yield return MoveToTargetPosition(targetPosition);
        yield return SmoothDescentToGround(targetPosition);
        pController.ResetFallSound();
        AudioManager.Instance.HandleStopSound(soundEvent, true);
        pController.SetVaulting(false);
    }*/

    private IEnumerator PerformVault(Vector3 targetPosition, VaultOptions vaultOption)
    {
        pController.SetVaulting(true);
        
        if (vaultOption == VaultOptions.OneVault)
        {
            cameraAnimator.SetTrigger("StartVault");
            yield return new WaitForSeconds(0.3f);
        }

        string soundEvent = GetVaultSoundEvent();
        AudioManager.Instance.HandlePlaySound3D(soundEvent, transform.position);

        yield return MoveToTargetPosition(targetPosition, vaultOption);
        yield return SmoothDescentToGround(targetPosition);
        pController.ResetFallSound();
        AudioManager.Instance.HandleStopSound(soundEvent, true);
        pController.SetVaulting(false);
    }

    private string GetVaultSoundEvent()
    {
        string soundEvent;

        switch (currentVaultObject.VaultOption)
        {
            case VaultOptions.OneVault:
                soundEvent = "event:/Test/TestLoop";
                break;

            case VaultOptions.CloseOne:
                soundEvent = "event:/Test/TestOneShot";
                break;

            case VaultOptions.FurtherOne:
                soundEvent = "event:/Jugador/jugador_escalar_paret";
                break;

            default:
                soundEvent = "event:/Jugador/jugador_escalar_paret";
                break;
        }

        return soundEvent;
    }

    private IEnumerator SmoothDescentToGround(Vector3 targetPosition)
    {
        if (Physics.Raycast(targetPosition + Vector3.up * raycastStartHeight, Vector3.down, out RaycastHit hit, raycastMaxDistance, groundMask))
        {
            Vector3 landingPos = hit.point;
            float descendElapsed = 0f;
            Vector3 from = transform.position;

            while (descendElapsed < descendTime)
            {
                float t = descendElapsed / descendTime;
                transform.position = Vector3.Lerp(from, landingPos, t);
                descendElapsed += Time.deltaTime;
                yield return null;
            }

            transform.position = landingPos;
        }
        else
        {
            transform.position = targetPosition;
        }
    }

    private IEnumerator MoveToTargetPosition(Vector3 targetPosition, VaultOptions vaultOption)
    {
        Vector3 startPosition = transform.position;
        float elapsedTime = 0f;
        float duration = Vector3.Distance(startPosition, targetPosition) / vaultSpeed;

        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        if(vaultOption == VaultOptions.OneVault)
            cameraAnimator.SetTrigger("StopVault");
        
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * vaultCheckRayDistance);
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
