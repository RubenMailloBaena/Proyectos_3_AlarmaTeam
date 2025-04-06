using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerController))]
public class PlayerVaultController : MonoBehaviour
{
    private PlayerController pController;
    
    [SerializeField] private InputActionReference jumpInput;

    [Header("Vault Attributes")] 
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
        if (pController.IsVaulting || pController.IsUsingVision) return;
        
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, vaultCheckRayDistance, vaultLayer))
        {
            if (jumpInput.action.triggered) //Perform Vault
            {
                if (pController.IsCrouching)
                {
                    pController.TryVaultCrouched();
                    return;
                }
                
                if (hit.transform.TryGetComponent(out ICanVault vaultObject))
                    StartCoroutine(PerformVault(vaultObject.GetVaultEndPoint(transform.position)));
            }
        }
    }

    private IEnumerator PerformVault(Vector3 targetPosition)
    {
        pController.SetVaulting(true);

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
