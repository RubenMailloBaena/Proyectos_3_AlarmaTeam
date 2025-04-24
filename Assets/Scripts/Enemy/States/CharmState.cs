using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharmState : State
{
    [Header("Charm Attributes")] 
    [SerializeField] private float charmSpeed = 2f;
    [SerializeField] private float rotateSpeed = 3f;
    private Vector3 targetPos, direction;
    private bool setDestination;

    [Header("STATES")] 
    public CheckState checkState;

    public Material material;
    
    public override void InitializeState()
    {
        eController.HideArrow();
        eController.ManualRotation(false);
        eController.ChangeMaterial(material);
        eController.StopAgent();
        eController.SetAgentSpeed(charmSpeed);
        setDestination = false;
    }

    public override State RunCurrentState()
    {
        targetPos = eController.GetLeverPosition();
        targetPos.y = transform.position.y;
        direction = (targetPos - transform.position).normalized;
        
        if (eController.RotateEnemy(direction, rotateSpeed))
        {
            if (!setDestination)
            {
                setDestination = true;
                eController.GoToLever();
            }

            if (eController.ArrivedToPosition(targetPos))
            {
                eController.InteractLever();
                eController.SetCharmLockedVisual(false);
                return checkState;
            }
        }
        return this;
    }
}
