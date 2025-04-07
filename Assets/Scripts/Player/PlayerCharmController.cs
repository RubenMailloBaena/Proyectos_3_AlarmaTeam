using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerController))]
public class PlayerCharmController : MonoBehaviour
{
    private PlayerController pController;

    [SerializeField] private InputActionReference charmInput;
    [SerializeField] private InputActionReference attackInput;
    [SerializeField] private Transform leanParent;
    
    [Header("Charm Attributes")]
    [SerializeField] private float charmRange = 25f;

    [Header("Visual circle")]
    [SerializeField] private GameObject visionCircle;

    private RawImage charmImage;
    private IEnemyInteractions mouseTarget;
    private IEnemyInteractions lockedTarget;
    private List<IInteractable> interactables;
    private bool inCharmingMode;

    private void Awake()
    {
        pController = GetComponent<PlayerController>();
    }

    private void Start()
    {
        charmImage = GameManager.GetInstance().GetPlayerCharmingImage();
        if (visionCircle != null)
        {
            float diameter = charmRange * 2f;
            float scaleFactor = diameter / 10f;
            visionCircle.transform.localScale = new Vector3(scaleFactor, 1f, scaleFactor);
        }
    }

    private void Update()
    {
        GatherInput();
        CharmingMode();
        UpdateLockedTarget();
        CharmedTargetLogic();
    }

    private void showCircle(bool show)
    {
        if (visionCircle != null)
            visionCircle.SetActive(show);
    }

    private void GatherInput()
    {
        if (charmInput.action.triggered)
        {
            SwapMode();

            if (!inCharmingMode)
            {
                ClearMouseTarget();
                ClearLockedTarget(); 
            }
        }

        if (attackInput.action.triggered)
        {
            if (lockedTarget == null && mouseTarget != null)
            {
                lockedTarget = mouseTarget;
                mouseTarget = null;
            }
        }
    }

    private void SwapMode()
    {
        inCharmingMode = !inCharmingMode;
        charmImage.enabled = inCharmingMode;
        showCircle(inCharmingMode);
    }

    private void CharmingMode()
    {
        if (!inCharmingMode || lockedTarget != null)
        {
            ClearMouseTarget();
            return;
        }

        if (Physics.Raycast(leanParent.position, leanParent.forward, out RaycastHit hit, charmRange))
        {
            if (hit.transform.TryGetComponent(out IEnemyInteractions enemy))
            {
                if (enemy != mouseTarget)
                {
                    ClearMouseTarget();
                    mouseTarget = enemy;
                    mouseTarget.SetTargetVisual(true);
                }
            }
            else
                ClearMouseTarget();
        }
        else
            ClearMouseTarget();
    }

    private void UpdateLockedTarget()
    {
        if (lockedTarget != null)
        {
            float distance = Vector3.Distance(transform.position, lockedTarget.GetPosition());
            if (distance > charmRange || !inCharmingMode || lockedTarget.IsInChaseOrAttack())
            {
                SwapMode();
                ClearLockedTarget();
            }
        }
    }

    private void ClearMouseTarget()
    {
        if (mouseTarget != null)
        {
            mouseTarget.SetTargetVisual(false);
            mouseTarget = null;
        }
    }

    private void ClearLockedTarget()
    {
        if (lockedTarget != null)
        {
            lockedTarget.ClearIntarectables();
            interactables.Clear();
            lockedTarget.SetTargetVisual(false);
            lockedTarget = null;
        }
    }

    private void CharmedTargetLogic()
    {
        if (lockedTarget != null)
        {
            interactables = lockedTarget.ActivateIntarectables();
            if (Physics.Raycast(leanParent.position, leanParent.forward, out RaycastHit hit))
            {
                if (hit.transform.TryGetComponent(out IInteractable lever))
                {
                    if (attackInput.action.triggered && interactables.Contains(lever))
                    {
                        lockedTarget.SetCharmedState(lever);
                        ClearLockedTarget();
                        SwapMode();
                    }
                }
            }
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
        attackInput.action.Enable();
    }

    private void OnDisable()
    {
        charmInput.action.Disable();
        attackInput.action.Disable();
    }
}
