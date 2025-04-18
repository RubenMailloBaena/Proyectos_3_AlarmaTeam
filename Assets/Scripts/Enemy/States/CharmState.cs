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
        eController.Renderer.ChangeMaterial(material);
        eController.Movement.StopAgent();
        eController.Movement.SetAgentSpeed(charmSpeed);
        targetPos = eController.Charm.GetLeverPosition();
        targetPos.y = transform.position.y;
        direction = (targetPos - transform.position).normalized;
        setDestination = false;
    }

    public override State RunCurrentState()
    {
        if (eController.Movement.RotateEnemy(direction, rotateSpeed))
        {
            if (!setDestination)
            {
                setDestination = true;
                eController.Movement.GoToLever();
            }

            if (eController.Movement.ArrivedToPosition(targetPos))
            {
                eController.Charm.InteractLever();
                eController.Charm.SetLockedVisual(false);
                return checkState;
            }
        }
        return this;
    }
}
