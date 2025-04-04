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
    private float distanceToPlayer;
    
    [Header("STATES")]
    public CheckState checkState;
    public ChaseState chaseState;
    
    public override void InitializeState()
    {
        eController.SetAgentSpeed(seenSpeed);
        currentFillValue = baseFillPerSecond;
    }

    public override State RunCurrentState()
    {
        distanceToPlayer = eController.GoToPlayerPosition();
        
        if (distanceToPlayer <= eController.minViewDistance || currentFillValue >= barMaxCapacity)
            return chaseState;

        if (currentFillValue <= 0.0f)
            return checkState;

        if (distanceToPlayer <= eController.maxViewDistance)
        {
            float normalizedDistance = 1f - Mathf.Clamp01(distanceToPlayer / eController.maxViewDistance);
            float fillRate = Mathf.Lerp(baseFillPerSecond, maxFillPerSecond, normalizedDistance);

            currentFillValue += fillRate * Time.deltaTime;
        }
        else
            currentFillValue -= substractPerSecond * Time.deltaTime;
        return this;
    }
}
