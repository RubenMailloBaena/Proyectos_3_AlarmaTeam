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
    private Vector3 playerPos, playerDir;
    private bool isTurning = false;
    
    [Header("STATES")]
    public CheckState checkState;
    public ChaseState chaseState;

   public override void InitializeState()
    {
        eController.SetAnimation(AnimationType.Idle, false);
        eController.StopSound();
        
        if (eController.isChasingPlayer)
        {
            eController.SwitchToNextState(chaseState);
            return;
        }

        eController.ActivateSeenArrow();
        eController.ManualRotation(true);
        eController.SetAgentSpeed(seenSpeed);
        currentFillValue = baseFillPerSecond;
        isTurning = false;
    }

    public override State RunCurrentState()
    {
        if (eController.IsPlayerInVision())
        {
            playerPos = eController.GoToPlayerPosition();
            float distanceToPlayer = Vector3.Distance(playerPos, transform.position);
            playerDir = (playerPos - transform.position).normalized;

            if (!eController.RotateEnemy(playerDir, seenRotationSpeed))
            {
                if (!isTurning)
                {
                    Vector3 cross = Vector3.Cross(transform.forward, playerDir);
                    if (cross.y > 0)
                        eController.SetAnimation(AnimationType.TurnRight, false);
                    else if (cross.y < 0)
                        eController.SetAnimation(AnimationType.TurnLeft, false);

                    isTurning = true;
                }
            }
            else
            {
                if (isTurning)
                {
                    eController.SetAnimation(AnimationType.StopTurn, false);
                    isTurning = false;
                }
            }

            fillRate = MapFunction(distanceToPlayer, eController.GetMaxViewDis(), eController.GetMinViewDis(), baseFillPerSecond, maxFillPerSecond);
            currentFillValue += fillRate * Time.deltaTime;

            if (currentFillValue >= barMaxCapacity)
            {
                eController.ManualRotation(false);
                return chaseState;
            }
        }
        else
        {
            if (isTurning)
            {
                eController.SetAnimation(AnimationType.StopTurn, false);
                isTurning = false;
            }

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
