using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerController))]
public class PlayerInteractController : MonoBehaviour
{
    private PlayerController pController;
    
    [SerializeField] private InputActionReference attackInput;
    [SerializeField] private Transform leanParent;
    
    [Header("Interact Attributes")] 
    [SerializeField] private float interactObjectDistance = 30f;
    
    private IInteractable target;

    private void Awake()
    {
        pController = GetComponent<PlayerController>();
    }
    
    private void Update()
    {
        CanInteract();
        PerformInteraction();
    }
    
    private void CanInteract()
    {
        if (Physics.Raycast(leanParent.position, leanParent.forward, out RaycastHit hit, interactObjectDistance))
        {
            if (hit.transform.TryGetComponent(out IInteractable interactable))
            {
                float distance = Vector3.Distance(hit.point, transform.position);
                if (distance <= interactable.InteractDistance)
                {
                    target = interactable;
                    target.SelectObject(true);
                }
                else ClearTarget();
            }
            else 
                ClearTarget();
        }
        else
            ClearTarget();
    }
    
    private void ClearTarget()
    {
        if (target != null)
        {
            target.SelectObject(false);
            target = null;
        }
    }
    
    private void PerformInteraction()
    {
        if (target != null && attackInput.action.triggered)
        {
            target.Interact();
            ClearTarget();
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(leanParent.position, leanParent.forward * interactObjectDistance);
    }

    private void OnEnable()
    {
        attackInput.action.Enable();
    }

    private void OnDisable()
    {
        attackInput.action.Disable();
    }
}
