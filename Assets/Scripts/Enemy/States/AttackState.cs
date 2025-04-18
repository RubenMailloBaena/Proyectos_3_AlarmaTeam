using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AttackState : State
{
    [Header("Attack Attributes")]
    [SerializeField] private float priestSpeed = 1.5f;
    [SerializeField] private float rotationSpeed = 5f;

    public float damage;

    [Header("STATES")] 
    public ChaseState chaseState;

    private Vector3 playerPos;
    private PlayerController pController;

    public Material material;
    
    public override void InitializeState()
    {
        eController.ActivateExclamation();
        eController.Renderer.ChangeMaterial(material);
        eController.Movement.ManualRotation(true);
        eController.isChasingPlayer = false;
        pController = GameManager.GetInstance().GetPlayerController();
        
        if (eController.enemyType == EnemyType.Knight)
            StartCoroutine(KillPlayer());
        else
        {
            eController.Movement.SetAgentSpeed(priestSpeed);
            playerPos = eController.Movement.GoToPlayerPosition();
        }
    }

    public override State RunCurrentState()
    {
        playerPos = eController.Movement.GoToPlayerPosition();
        float distanceToPlayer = Vector3.Distance(transform.position, playerPos);

        Vector3 dir = (playerPos - transform.position).normalized;
        eController.Movement.RotateEnemy(dir, rotationSpeed);
        
        if (eController.Vision.isPlayerInVision && distanceToPlayer <= eController.Vision.GetAttackDis())
        {
            eController.Renderer.SetLight(true);
            pController.TakeDamage(damage);
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
        eController.Movement.StopAgent();

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
