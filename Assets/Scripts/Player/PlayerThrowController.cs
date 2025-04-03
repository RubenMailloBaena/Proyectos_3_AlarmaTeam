using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerController))]
public class PlayerThrowController : MonoBehaviour
{
    [SerializeField] private InputActionReference attackInput;
    [SerializeField] private Transform leanParent;

    [Header("Throw Objects Attributes")] 
    [SerializeField] private float interactDistance = 10f;

    private IThrowableObject target;

    private void Update()
    {
        CanInteract();
        PerformInteraction();
    }

    private void CanInteract()
    {
        if (Physics.Raycast(leanParent.position, leanParent.forward, out RaycastHit hit, interactDistance))
        {
            if (hit.transform.TryGetComponent(out IThrowableObject tObject))
            {
                target = tObject;
                target.SelectObject(true);
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
            target.ThrowObject();
            ClearTarget();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(leanParent.position, leanParent.forward * interactDistance);
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
