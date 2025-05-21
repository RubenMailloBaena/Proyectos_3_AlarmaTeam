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

            if (jumpInput.action.triggered && !pController.IsGamePaused) //Perform Vault
            {
                if (pController.IsCrouching && standUpWhenVaulting)
                {
                    pController.TryVaultCrouched();
                    return;
                }
                
                if (hit.transform.TryGetComponent(out ICanVault vaultObject))
                    StartCoroutine(PerformVault(vaultObject.GetVaultEndPoint(transform.position)));
            }

            pController.CanInteract(jumpInput, InputType.Press, this, ActionType.Vault);
        }
        else
        {
            pController.HideInteract(this);
            pController.SetCanVault(false);
        }
            
    }

    private IEnumerator PerformVault(Vector3 targetPosition)
    {
        pController.SetVaulting(true);
        AudioManager.Instance.HandlePlaySound3D("event:/Jugador/jugador_escalar_paret", transform.position);

        Vector3 startPosition = transform.position;
        float elapsedTime = 0f;
        float duration = Vector3.Distance(startPosition, targetPosition) / vaultSpeed; 

        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null; 
        }

        transform.position = targetPosition;
        AudioManager.Instance.HandleStopSound("event:/Jugador/jugador_escalar_paret",true);
        pController.SetVaulting(false);
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
