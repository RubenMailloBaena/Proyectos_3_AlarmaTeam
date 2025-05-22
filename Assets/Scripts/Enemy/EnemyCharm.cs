using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCharm : MonoBehaviour, ICharmEnemy
{
    private EnemyController eController;
    
    [SerializeField] private float interactDistance = 10f;
    private IInteractable targetLever;

    public void SetCharm(EnemyController enemyController)
    {
        eController = enemyController;
    }
    
    public void SetTargetVisual(bool active)
    {
        if (eController.IsAttacking())
            eController.ChangeOutline(false);
        else
            eController.ChangeOutline(active);
    }

    public bool IsInChaseOrAttack()
    {
        if (eController.IsAttacking() || eController.IsChasing())
        {
            SetTargetVisual(false);
            return true;
        }
        return false;
    }

    public List<IInteractable> ActivateIntarectables()
    {
        List<IInteractable> interactables = new List<IInteractable>();
        foreach (IInteractable lever in GameManager.GetInstance().GetInteractables())
        {
            float distance = Vector3.Distance(transform.position, lever.GetPosition());

            if (distance <= interactDistance)
            {
                lever.SelectObject(true);
                interactables.Add(lever);
            }
            else
                lever.SelectObject(false);
        }

        return interactables;
    }

    public void ClearIntarectables()
    {
        foreach (IInteractable lever in GameManager.GetInstance().GetInteractables())
            lever.SelectObject(false);
    }

    public void SetCharmedState(IInteractable lever)
    {
        targetLever = lever;
        SetLockedVisual(true);
        eController.SetPositionBeforeMoving();
        eController.SwitchToCharmState();
        SetTargetVisual(false);
    }


    public void SetLockedVisual(bool active)
    {
        targetLever.isLocked = active;
        eController.SetLocked(active);
    }

    public bool IsCharmed() => eController.IsCharmed();

    public Vector3 GetPosition() => transform.position;

    public Vector3 GetLeverPosition() => targetLever.GetPosition();

    public void InteractLever() => targetLever.Interact();

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, interactDistance);
    }
}
