using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyType{
    Priest,
    Knight
}
public class EnemyController : MonoBehaviour, ICanHear
{
    private State currentState, nextState, lastState;
    
    private NavMeshAgent meshAgent;
    private PlayerController pController;

    [Header("DEBUG TEXT")]
    [SerializeField] private TextMeshProUGUI debugText;

    [Header("STATES")]
    [SerializeField] private IdleState idleState;
    [SerializeField] private HearState hearState;
    [SerializeField] private LookAtState lookAtState;
    [SerializeField] private SeenState seenState;
    [SerializeField] private ChaseState chaseState;
    [SerializeField] private AttackState attackState;

    [Header("ENEMY OPTIONS")] 
    public bool isIdleEnemy;
    public EnemyType enemyType;
    [SerializeField] private GameObject lightSource;

    [Header("ENEMY VISION CONE")] 
    [SerializeField] private float viewAngle = 60f;
    [SerializeField] public float maxViewDistance = 15f;   
    [SerializeField] public float minViewDistance = 10f; 
    [SerializeField] public float attackDistance = 5f; 
    [Tooltip("los grados minimos para que cuando rotamos con lerp, llegue al target Rotation instantaneo. (si quedan 5 graods para llegar, hara TP a la rotacion final)")]
    [SerializeField] private float minAngleDiffToRotate = 5f;    
    [SerializeField] private Vector3 enemyEyesOffset = new Vector3(0f, 1f, 0f);
    
    private Vector3 enemyPos;
    
    [Header("CANT SEE THROUGH")]
    public LayerMask groundLayer;

    [Header("WAY POINTS")] 
    [SerializeField] private List<Waypoint> waypoints;
    private int waypointIndex;
    private float minDistanceToArrive = 0.1f;
    
    //VARIABLES
    [HideInInspector] public bool soundWasAnObject = true;
    [HideInInspector] public bool inPlayerHearState = true;
    [HideInInspector] public bool isPlayerInVision;
    [HideInInspector] public bool ignorePlayerInMinVision;
    [HideInInspector] public bool isChasingPlayer;
    [HideInInspector] public bool killingPlayer;
    [HideInInspector] public Vector3 soundPos;
    [HideInInspector] public Vector3 enemyPosBeforeMoving;
    [HideInInspector] public float distanceToPlayer;

    void Awake()
    {
        meshAgent = GetComponent<NavMeshAgent>();
        SwitchToNextState(idleState);
    }

    void Start()
    {
        pController = GameManager.GetInstance().GetPlayerController();
        inPlayerHearState = true;
        enemyPosBeforeMoving = Vector3.zero;
    }

    void Update()
    {
        RunStateMachine();
        CanSeePlayer();
    }

    private void RunStateMachine()
    {
        if (killingPlayer) return;
        
        nextState = currentState?.RunCurrentState();

        if (nextState != null && nextState != currentState)
            SwitchToNextState(nextState);
    }

    public void SwitchToNextState(State nextState)
    {
        if (killingPlayer) return;
        
        //DEBUG ONLY
        debugText.text = nextState.name;
        //
        lastState = currentState;
        currentState = nextState;
        currentState?.SetReference(this);
        currentState?.InitializeState();
    }

    private void CanSeePlayer()
    {
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
            SetPositionBeforeMoving();
            
            if(distanceToPlayer <= attackDistance && currentState != attackState)
                SwitchToNextState(attackState);
            else if(distanceToPlayer <= minViewDistance && distanceToPlayer > attackDistance && currentState != chaseState)
                SwitchToNextState(chaseState);
            else if(!ignorePlayerInMinVision && distanceToPlayer > minViewDistance && currentState != seenState)
                SwitchToNextState(seenState); 
        }
    }
    
    private void SetPositionBeforeMoving()
    {
        if (enemyPosBeforeMoving == Vector3.zero)
            enemyPosBeforeMoving = transform.position;
    }

    //----------------------------ICanHear FUNCTIONS-----------------------------
    
    public void HeardSound(Vector3 soundPoint, bool isObject)
    {
        if (!isObject && Mathf.Abs(soundPoint.y - transform.position.y) > 0.3f || isChasingPlayer || currentState == attackState) 
            return;

        SetPositionBeforeMoving();
        
        soundPos = soundPoint;
        soundPos.y = transform.position.y;

        if (!soundWasAnObject && !isObject && !inPlayerHearState) 
        {
            SwitchToNextState(lookAtState);
            return;
        }

        soundWasAnObject = isObject;
        SwitchToNextState(hearState);
    }
    
    //---------------------------GENERAL FUNCTIONS-------------------------------

    public void SetAgentSpeed(float speed)
    {
        if (isChasingPlayer) return;
        meshAgent.speed = speed;
    }

    public void ReturnToLastState() => SwitchToNextState(lastState);
    public void StopAgent() => meshAgent.ResetPath();
    public void SetLight(bool active) => lightSource.SetActive(active);

    public Vector3 GetEnemyVelocity() => meshAgent.velocity;

    public void ManualRotation(bool active)
    {
        meshAgent.updateRotation = active;
    }

    public float GetWaitTime()
    {
        if (waypoints.Count == 0) 
            return 0.0f;
        return waypoints[waypointIndex].waitTime;
    }
   
    public Vector3 GoToWaypoint()
    {
        Vector3 pos = waypoints[waypointIndex].Position();
        meshAgent.SetDestination(pos);
        return pos;
    }

    public Vector3 GoToSoundSource()
    {
        meshAgent.SetDestination(soundPos);
        return soundPos;
    }

    public Vector3 GoToPreviousPosition()
    {
        meshAgent.SetDestination(enemyPosBeforeMoving);
        return enemyPosBeforeMoving;
    }

    public Vector3 GoToPlayerPosition()
    {
        meshAgent.SetDestination(pController.GetPlayerPosition());
        return pController.GetPlayerPosition();
    }

    public Vector3 GetLookDirection()
    {
        if (!waypoints[waypointIndex].rotateEnemy) 
            return Vector3.zero;

        return waypoints[waypointIndex].Direction();
    }

    public bool RotateEnemy(Vector3 lookDir, float rotationSpeed)
    {
        Quaternion targetDir = Quaternion.LookRotation(lookDir);
        
        //Miramos si ya estamos alineados 
        float angleDiff = Quaternion.Angle(transform.rotation, targetDir);
        if (angleDiff < minAngleDiffToRotate)
        {
            transform.rotation = targetDir;
            return true;
        }
        
        //Si aun nos queda por girar seguimos 
        transform.rotation = Quaternion.Slerp(transform.rotation, targetDir, Time.deltaTime * rotationSpeed);
        return false;
    }
   
    public bool ArrivedToPosition(Vector3 position)
    {
        return Vector3.Distance(transform.position, position) <= minDistanceToArrive;
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
    
    public void IncrementIndex()
    {
        waypointIndex++;
        if (waypointIndex >= waypoints.Count)
            waypointIndex = 0;
    }

    public float GetPathLegth(Vector3 targetPos)
    {
        NavMeshPath path = new NavMeshPath();

        if (meshAgent.CalculatePath(targetPos, path) && path.status == NavMeshPathStatus.PathComplete) //Exists a path
        {
            float pathLength = 0.0f;
            for (int i = 1; i < path.corners.Length; i++)
            {
                pathLength += Vector3.Distance(path.corners[i - 1], path.corners[i]);
            }
            Debug.LogWarning("Distance to sound: " + pathLength);
            return pathLength;
        }
        return -1f; //No path available
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
}



[Serializable]
public class Waypoint
{
    public bool rotateEnemy;
    public float waitTime = 0.0f;
    public Transform waypoint;
    public Vector3 Position() => waypoint.position;
    public Vector3 Direction() => waypoint.forward;
}
