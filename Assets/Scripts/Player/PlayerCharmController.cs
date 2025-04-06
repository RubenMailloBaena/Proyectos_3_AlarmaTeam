using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerController))]
public class PlayerCharmController : MonoBehaviour
{
    private PlayerController pController;

    [SerializeField] private InputActionReference charmInput;
    [SerializeField] private Transform leanParent;
    
    [Header("Charm Attributes")]
    [SerializeField] private float charmRange = 25f;

    private IEnemyInteractions target;
    private bool inCharmingMode;

    private void Awake()
    {
        pController = GetComponent<PlayerController>();
    }

    private void Update()
    {
        GatherInput();
        CharmingMode();
    }

    private void GatherInput()
    {
        if (charmInput.action.triggered)
            inCharmingMode = !inCharmingMode;
    }

    private void CharmingMode()
    {
        if (inCharmingMode)
        {
            if (Physics.Raycast(leanParent.position, leanParent.forward, out RaycastHit hit, charmRange))
            {
                if (hit.transform.TryGetComponent(out IEnemyInteractions enemy))
                {
                    target = enemy;
                    target.SetTargetVisual(true);
                }
                else ClearTarget();
            }
            else ClearTarget();
        }
        else ClearTarget();
    }
    
    private void ClearTarget()
    {
        if (target != null)
        {
            target.SetTargetVisual(false);
            target = null;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, charmRange);
    }

    private void OnEnable()
    {
        charmInput.action.Enable();
    }

    private void OnDisable()
    {
        charmInput.action.Disable();
    }
}
