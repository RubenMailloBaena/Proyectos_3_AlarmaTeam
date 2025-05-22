using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeenState : State
{
    
    [Header("SEEN ATTRIBUTES")]
    [SerializeField] private float seenSpeed = 1f;
    [SerializeField] private float seenRotationSpeed = 5f;
    [SerializeField] private float substractPerSecond = 2f;
    [SerializeField] private float baseFillPerSecond = 5f;
    [SerializeField] private float maxFillPerSecond = 10f;
    [SerializeField] private float barMaxCapacity = 100f;
    private float currentFillValue = 0f;
    private float fillRate = 0f;
    private Vector3 playerPos;
    
    [Header("STATES")]
    public CheckState checkState;
    public ChaseState chaseState;

    public override void InitializeState()
    {
        //TODO: CHANGE IN FUTURE
        eController.SetAnimation(AnimationType.Idle, false);
        
        if (eController.isChasingPlayer)
        {
            eController.SwitchToNextState(chaseState);
            return;
        }
        eController.ActivateSeenArrow();
        eController.ManualRotation(true);
        eController.SetAgentSpeed(seenSpeed);
        currentFillValue = baseFillPerSecond;
    }

    public override State RunCurrentState()
    {
        if (eController.IsPlayerInVision())
        {
            playerPos = eController.GoToPlayerPosition();
            float distanceToPlayer = Vector3.Distance(playerPos, transform.position);

            Vector3 dir = (playerPos - transform.position).normalized;
            eController.RotateEnemy(dir, seenRotationSpeed);

            fillRate = MapFunction(distanceToPlayer, eController.GetMaxViewDis(), eController.GetMinViewDis(),baseFillPerSecond,maxFillPerSecond);
            currentFillValue += fillRate * Time.deltaTime;

            if (currentFillValue >= barMaxCapacity)
            {
                return chaseState;
            }
        }
        else
        {
            currentFillValue -= substractPerSecond * Time.deltaTime;

            if (currentFillValue <= 0.0f)
            {
                eController.HideArrow();
                eController.ManualRotation(false);
                return checkState;
            }
        }
        
        eController.UpdateSeenAmount(currentFillValue, barMaxCapacity);
        return this;
    }
    
    public float MapFunction(float value, float fromMin, float fromMax, float toMin, float toMax)
    {
        return (value - fromMin) / (fromMax - fromMin) * (toMax - toMin) + toMin;
    }
}
