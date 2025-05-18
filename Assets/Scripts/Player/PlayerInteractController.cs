using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerController))]
public class PlayerInteractController : MonoBehaviour, IPlayerComponent
{
    private PlayerController pController;
    
    [SerializeField] private InputActionReference attackInput;
    [SerializeField] private InputActionReference pauseInput;
    [SerializeField] private Transform leanParent;
    
    [Header("Interact Attributes")] 
    [SerializeField] private float interactObjectDistance = 30f;
    [SerializeField] private LayerMask allLayer;
    
    private IInteractable target;

    private void Awake()
    {
        pController = GetComponent<PlayerController>();
    }
    
    private void Update()
    {
        CanInteract();
        PerformInteraction();
        SetPauseMenu();
    }
    
    private void CanInteract()
    {
        if (Physics.Raycast(leanParent.position, leanParent.forward, out RaycastHit hit, interactObjectDistance, allLayer))
        {
            if (hit.transform.TryGetComponent(out IInteractable interactable))
            {
                float distance = Vector3.Distance(hit.point, transform.position);
                if (distance <= interactable.InteractDistance)
                {
                    if (target != null && target != interactable) //SI PASAMOS DE UNO A OTRO DIRECTAMENTE
                        ClearTarget();
                    
                    target = interactable;
                    target.SelectObject(true);
                    if(interactable.canInteract)
                        pController.CanInteract(attackInput, InputType.Press, this);
                }
                else ClearTarget();
            }
            else 
                ClearTarget();
        }
        else ClearTarget();
    }
    
    private void ClearTarget()
    {
        if (target != null)
        {
            target.SelectObject(false);
            pController.HideInteract(this);
            target = null;
        }
    }
    
    private void PerformInteraction()
    {
        if (target != null && attackInput.action.triggered && !pController.IsGamePaused)
        {
            target.Interact();
            ClearTarget();
        }
    }

    private void SetPauseMenu()
    {
        if(pauseInput.action.triggered && !pController.IsPlayerDead && !pController.IsVaulting)
            pController.SetPauseMenu();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(leanParent.position, leanParent.forward * interactObjectDistance);
    }

    private void OnEnable()
    {
        attackInput.action.Enable();
        pauseInput.action.Enable();
    }

    private void OnDisable()
    {
        attackInput.action.Disable();
        pauseInput.action.Disable();
    }
}