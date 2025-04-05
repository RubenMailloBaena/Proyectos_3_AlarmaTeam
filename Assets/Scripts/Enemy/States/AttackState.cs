using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AttackState : State
{
    [Header("Attack Attributes")]
    [SerializeField] private float priestSpeed = 1.5f;

    [Header("STATES")] 
    public ChaseState chaseState;

    private Vector3 playerPos;
    private PlayerController pController;
    
    public override void InitializeState()
    {
        eController.isChasingPlayer = false;
        pController = GameManager.GetInstance().GetPlayerController();
        
        if (eController.enemyType == EnemyType.Knight)
            StartCoroutine(KillPlayer());
        else
        {
            eController.SetAgentSpeed(priestSpeed);
            playerPos = eController.GoToPlayerPosition();
        }
    }

    public override State RunCurrentState()
    {
        playerPos = eController.GoToPlayerPosition();
        float distanceToPlayer = Vector3.Distance(transform.position, playerPos);
        
        if (eController.isPlayerInVision && distanceToPlayer <= eController.attackDistance)
        {
            eController.SetLight(true);
            if (pController.TakeDamage())
                StartCoroutine(KillPlayer());
            return this;
        }
        return chaseState;
    }

    private IEnumerator KillPlayer()
    {
        eController.killingPlayer = true;
        GameManager.GetInstance().GetPlayerController().SetIsPlayerDead(true);
        eController.StopAgent();
        
        Debug.LogError("PLAYER DEAD");
        yield return null;
    }
}
