using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyVision : MonoBehaviour
{
    private EnemyController eController;
    private PlayerController pController;
    
    [Header("ENEMY VISION CONE")] 
    [SerializeField] private float viewAngle = 60f;
    [SerializeField] private float maxViewDistance = 15f;   
    [SerializeField] private float minViewDistance = 10f; 
    [SerializeField] private float attackDistance = 5f; 
    [Tooltip("los grados minimos para que cuando rotamos con lerp, llegue al target Rotation instantaneo. (si quedan 5 graods para llegar, hara TP a la rotacion final)")]
    [SerializeField] private Vector3 enemyEyesOffset = new Vector3(0f, 1f, 0f);
    private Vector3 enemyPos;
    
    [Header("CANT SEE THROUGH")]
    public LayerMask groundLayer;
    
    private float distanceToPlayer;
    private bool isPlayerInVision;
    private bool ignorePlayerInMinVision;

    public void SetVision(EnemyController enemyController)
    {
        eController = enemyController;
    }

    private void Start()
    {
        pController = GameManager.GetInstance().GetPlayerController();
    }

    private void Update()
    {
        CanSeePlayer();
    }

    private void CanSeePlayer()
    {
        if (eController.IsCharmed()) return;
        
        Vector3 playerEyes = pController.GetPlayerEyesPosition();
        enemyPos = transform.position + enemyEyesOffset;
        distanceToPlayer = Vector3.Distance(enemyPos, playerEyes);

        isPlayerInVision = false; 

        //JUGADOR DENTRO DEL RANGO MINIMO DEL ENEMIGO
        if (distanceToPlayer > maxViewDistance)
            return;

        Vector3 directionToPlayer = (playerEyes - enemyPos).normalized;

        //EL JUGADOR ESTA DENTRO DE NUESTRO CONO DE VISION
        if (Vector3.Angle(transform.forward, directionToPlayer) > viewAngle / 2f)
            return;

        //SI NO HAY PAREDES EN MEDIO, ESTAMOS VIENDO AL PLAYER
        if (!Physics.Raycast(enemyPos, directionToPlayer, distanceToPlayer, groundLayer))
        {
            isPlayerInVision = true;
            eController.SetPositionBeforeMoving();
            
            if(distanceToPlayer <= attackDistance && !eController.IsAttacking())
                eController.SwitchToAttackState();
            else if(distanceToPlayer <= minViewDistance && distanceToPlayer > attackDistance && !eController.IsChasing())
                eController.SwitchToChaseState();
            else if(!ignorePlayerInMinVision && distanceToPlayer > minViewDistance && !eController.IsChasing() 
                    && !eController.InSeenState() && !eController.IsAttacking())
                eController.SwitchToSeenState();
        }
    }
    
    public bool IsPointInVision(Vector3 targetPos)
    {
        Vector3 origin = transform.position + new Vector3(0, 1.5f, 0); 
        Vector3 target = targetPos + new Vector3(0, 1.5f, 0); 
    
        Vector3 dir = (target - origin).normalized;
        float distance = Vector3.Distance(origin, target);
        
        Debug.DrawRay(origin, dir * maxViewDistance, Color.green);

        //ESTA MAS LEJOS QUE EL RANGO DE VISION MAXIMO
        if (distance > maxViewDistance)
            return false;

        //SI ESTA FUERA DEL CONO DE VISION
        if (Vector3.Angle(transform.forward, dir) > viewAngle / 2f)
            return false;

        //NO HAY NADA ENTRE MEDIO
        return !Physics.Raycast(origin, dir, distance, groundLayer); 
    }
    
    private void OnDrawGizmosSelected()
    {
        //MAX DISTANCE
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, maxViewDistance);
        
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, minViewDistance);
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackDistance);
        
        //VISION CONE
        Vector3 leftBoundary = Quaternion.Euler(0, -viewAngle / 2f, 0) * transform.forward * maxViewDistance;
        Vector3 rightBoundary = Quaternion.Euler(0, viewAngle / 2f, 0) * transform.forward * maxViewDistance;

        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + leftBoundary);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary);
    }

    public float GetAttackDis() => attackDistance;
    public float GetMaxViewDis() => maxViewDistance;
    public float GetMinViewDis() => minViewDistance;
    
    public bool IsPlayerInVision
    {
        get => isPlayerInVision;
    }

    public bool IgnorePlayerInMinVision
    {
        set => ignorePlayerInMinVision = value;
    }
}
