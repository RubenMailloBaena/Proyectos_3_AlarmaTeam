using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRestart : MonoBehaviour, IRestartable
{
    private EnemyController eController;
    
    private Vector3 startingPos, checkpointPos;
    private Quaternion startingRotation, checkpointRotation;

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
    }
    
    public void RestartGame()
    {
        RestartInstructions();
        
        eController.ManualRotation(true);
        transform.position = startingPos;
        transform.rotation = startingRotation;
        eController.ManualRotation(false);

    }

    public void RestartFromCheckPoint()
    {
        RestartInstructions();
        
        eController.ManualRotation(true);
        transform.position = checkpointPos;
        transform.rotation = checkpointRotation;
        eController.ManualRotation(false);
    }

    private void RestartInstructions()
    {
        eController.StopKillCoroutine();
        eController.SetLight(false);
        eController.RestartIndex();
        eController.StopAgent();
        eController.SwitchToIdleState();
    }

    
    public void SetCheckPoint()
    {
        checkpointPos = transform.position;
        checkpointRotation = transform.rotation;
    }
}
