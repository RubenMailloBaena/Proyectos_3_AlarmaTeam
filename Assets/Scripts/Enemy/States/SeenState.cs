using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeenState : State
{
    [Header("SEEN ATTRIBUTES")]
    [SerializeField] private float seenSpeed = 1f;
    [SerializeField] private float substractPerSecond = 2f;
    [SerializeField] private float baseFillPerSecond = 5f;
    [SerializeField] private float maxFillPerSecond = 10f;
    [SerializeField] private float barMaxCapacity = 100f;
    private float currentFillValue = 0f;
    private float fillRate = 0f;
    private float distanceToPlayer;
    private bool firstTimeIn = false;
    
    [Header("STATES")]
    public CheckState checkState;
    public ChaseState chaseState;
    
    public override void InitializeState()
    {
        if (!firstTimeIn)
        {
            firstTimeIn = true;
            eController.SetAgentSpeed(seenSpeed);
            currentFillValue = baseFillPerSecond;
        }
    }

    public override State RunCurrentState()
    {
        if (eController.isPlayerInVision)
        {
            distanceToPlayer = eController.GoToPlayerPosition();

            fillRate = MapFunction(distanceToPlayer, eController.maxViewDistance, eController.minViewDistance,baseFillPerSecond,maxFillPerSecond);
            
            currentFillValue += fillRate * Time.deltaTime;

            if (distanceToPlayer <= eController.minViewDistance || currentFillValue >= barMaxCapacity)
            {
                firstTimeIn = false;
                return chaseState;
            }
        }
        else
        {
            currentFillValue -= substractPerSecond * Time.deltaTime;

            if (currentFillValue <= 0.0f)
            {
                firstTimeIn = false;
                return checkState;
            }
        }

        print(fillRate + " " + currentFillValue);
        return this;
    }
    
    public float MapFunction(float value, float fromMin, float fromMax, float toMin, float toMax)
    {
        return (value - fromMin) / (fromMax - fromMin) * (toMax - toMin) + toMin;
    }
}
