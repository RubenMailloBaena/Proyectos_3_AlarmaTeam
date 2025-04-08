using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AttackState : State
{
    [Header("Attack Attributes")]
    [SerializeField] private float priestSpeed = 1.5f;
    [SerializeField] private float rotationSpeed = 5f;

    [Header("STATES")] 
    public ChaseState chaseState;

    private Vector3 playerPos;
    private PlayerController pController;

    public Material material;
    
    public override void InitializeState()
    {
        eController.renderer.material = material;
        eController.ManualRotation(false);
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

        Vector3 dir = (playerPos - transform.position).normalized;
        eController.RotateEnemy(dir, rotationSpeed);
        
        if (eController.isPlayerInVision && distanceToPlayer <= eController.attackDistance)
        {
            eController.SetLight(true);
            pController.TakeDamage();
            if (pController.IsPlayerDead)
                StartCoroutine(KillPlayer());
            return this;
        }
        return chaseState;
    }

    private IEnumerator KillPlayer()
    {
        Transform playerTrans = GameManager.GetInstance().GetPlayerController().GetPlayerTransform();
        eController.killingPlayer = true;
        eController.StopAgent();

        Vector3 directionToEnemy = (transform.position - playerTrans.position).normalized;
        directionToEnemy.y = 0f;

        Quaternion targetRotation = Quaternion.LookRotation(directionToEnemy);
        float rotationSpeed = 5f;

        while (Quaternion.Angle(playerTrans.rotation, targetRotation) > 0.3f)
        {
            playerTrans.rotation = Quaternion.Slerp(playerTrans.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            yield return null;
        }
        playerTrans.rotation = targetRotation;

        Debug.LogError("PLAYER DEAD");
        yield return null;
    }
}
