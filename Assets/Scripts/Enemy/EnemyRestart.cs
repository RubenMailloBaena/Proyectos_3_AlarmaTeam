using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRestart : MonoBehaviour, IRestartable
{
    private EnemyController eController;
    
    private Vector3 startingPos, checkpointPos;
    private Quaternion startingRotation, checkpointRotation;
    private State checkpointState, startingState;
    private int checkpointIndex;
    private float checkpointWaitTime;
    private bool enemyWasDead;

    public void SetRestart(EnemyController enemyController)
    {
        eController = enemyController;
    }
    
    private void Start()
    {
        GameManager.GetInstance().AddRestartable(this);
        
        startingPos = transform.position;
        checkpointPos = startingPos;
        startingRotation = transform.rotation;
        checkpointRotation = startingRotation;
        startingState = eController.GetCurrentState();
        checkpointState = startingState;
        checkpointIndex = eController.GetIndex();
        checkpointWaitTime = eController.GetCurrentWaitTime();
    }
    
    public void RestartGame()
    {
        RestartInstructions();
        
        eController.enemyIsDead = false;
        eController.RestartIndex();
        eController.SetLockedVisual(false);
        eController.SwitchToNextState(startingState);

        eController.ManualRotation(true);
        eController.WarpAgent(startingPos);
        transform.rotation = startingRotation;
        eController.ManualRotation(false);

        checkpointPos = startingPos;
        checkpointRotation = startingRotation;
        enemyWasDead = false;
        checkpointIndex = 0;
        checkpointWaitTime = eController.GetCurrentWaitTime();
        checkpointState = startingState;
    }

    public void RestartFromCheckPoint()
    {
        RestartInstructions();

        eController.enemyIsDead = enemyWasDead;
        eController.SetIndex(checkpointIndex);
        eController.SwitchToNextState(checkpointState);
        eController.AddCurrentWaitTime(checkpointWaitTime);
        
        eController.ManualRotation(true);
        eController.WarpAgent(checkpointPos);
        transform.rotation = checkpointRotation;
        eController.ManualRotation(false);
    }

    private void RestartInstructions()
    {
        eController.exclamationShown = false;
        eController.StopAgent();
        eController.StopKillCoroutine();
        eController.SetLight(false);
        eController.SetRenderActive(!enemyWasDead);
    }
    
    public void SetCheckPoint()
    {
        checkpointState = eController.GetCurrentState();
        checkpointIndex = eController.GetIndex();
        checkpointWaitTime = eController.GetCurrentWaitTime();
        checkpointPos = transform.position;
        checkpointRotation = transform.rotation;
        enemyWasDead = eController.enemyIsDead;
    }

    private void OnDestroy() => GameManager.GetInstance().RemoveRestartable(this);

}
