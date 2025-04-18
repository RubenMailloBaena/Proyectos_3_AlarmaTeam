using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyType{
    Priest,
    Knight
}
public class EnemyController : MonoBehaviour, IEnemyInteractions, IVisible
{
    private State currentState, nextState, lastState;
    
    private PlayerController pController;
    private EnemySeenHUD eHUD;

    [Header("DEBUG TEXT")]
    [SerializeField] private TextMeshProUGUI debugText;

    [Header("REFERENCES")]
    [SerializeField] public Transform seenExclamationPos;
    [SerializeField] private GameObject weakSpotRenderer;
    [SerializeField] private GameObject heart;

    [Header("STATES")]
    [SerializeField] private IdleState idleState;
    [SerializeField] private HearState hearState;
    [SerializeField] private LookAtState lookAtState;
    [SerializeField] private SeenState seenState;
    [SerializeField] private ChaseState chaseState;
    [SerializeField] private AttackState attackState;
    [SerializeField] private CharmState charmState;

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
    
    //VARIABLES
    [HideInInspector] public bool soundWasAnObject = true;
    [HideInInspector] public bool inPlayerHearState = true;
    [HideInInspector] public bool isPlayerInVision;
    [HideInInspector] public bool ignorePlayerInMinVision;
    [HideInInspector] public bool isChasingPlayer;
    [HideInInspector] public bool killingPlayer;
    [HideInInspector] public bool exclamationShown;
    [HideInInspector] public Vector3 soundPos;
    [HideInInspector] public Vector3 enemyPosBeforeMoving;
    [HideInInspector] public float distanceToPlayer;
    
    //ENEMY COMPONENTS
    public EnemyMovement Movement { get; private set; }
    public EnemyRender Renderer { get; private set; }
    public EnemyCharm Charm { get; private set; }

    void Awake()
    {
        Movement = GetComponent<EnemyMovement>();
        Renderer = GetComponent<EnemyRender>();
        Charm = GetComponent<EnemyCharm>();

        Movement.SetMovement(this);
        Renderer.SetRenderer();
        Charm.SetCharm(this);
        
        SwitchToNextState(idleState);
    }

    void Start()
    {
        GameManager.GetInstance().GetPlayerController().AddEnemy(this);
        GameManager.GetInstance().GetPlayerController().AddVisible(this);
        pController = GameManager.GetInstance().GetPlayerController();
        eHUD = GameManager.GetInstance().GetEnemySeenHUD();
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
        debugText.text = nextState.name.ToUpper();
        //
        lastState = currentState;
        currentState = nextState;
        currentState?.SetReference(this);
        currentState?.InitializeState();
    }

    private void CanSeePlayer()
    {
        if (currentState == charmState) return;
        
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
            else if(!ignorePlayerInMinVision && distanceToPlayer > minViewDistance && currentState != chaseState && currentState != seenState)
                SwitchToNextState(seenState); 
        }
    }
    
    public void SetPositionBeforeMoving()
    {
        if (enemyPosBeforeMoving == Vector3.zero)
            enemyPosBeforeMoving = transform.position;
    }

    //----------------------------Hear FUNCTIONS-----------------------------
    
    public void HeardSound(Vector3 soundPoint, bool isObject)
    {
        if (!isObject && Mathf.Abs(soundPoint.y - transform.position.y) > 0.3f 
            || isChasingPlayer || currentState == attackState || currentState == charmState) 
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
    //----------------------------BackStab FUNCTIONS-----------------------------

    public void SetWeakSpot(bool active)
    {
        weakSpotRenderer.SetActive(active);
    }

    public void Backstab()
    {
        Destroy(gameObject);
    }
    
    //----------------------------MindControl FUNCTIONS-----------------------------
    
    public bool IsCharmed() => currentState == charmState;
    public bool IsAttacking() => currentState == attackState;
    public bool IsChasing() => currentState == chaseState;
    public void SwitchToCharmState() => SwitchToNextState(charmState);
    
    //----------------------------SEEN HUD FUNCTIONS-----------------------------
    public void ActivateSeenArrow()
    {
        eHUD.SetNewArrow(seenExclamationPos, gameObject.GetInstanceID());
    }
    public void UpdateSeenAmount(float amount, float maxAmount)
    {
        eHUD.UpdateArrowFillAmount(gameObject.GetInstanceID(), amount, maxAmount);
    }
    public void HideArrow()
    {
        if (lastState == seenState || currentState == seenState)
            eHUD.HideArrow(gameObject.GetInstanceID());
    }

    public void ShowExclamation()
    {
        eHUD.ShowExclamation(gameObject.GetInstanceID());
    }

    public void ActivateExclamation()
    {
        if (exclamationShown) return;
        exclamationShown = true;
        
        if (lastState != seenState)
            ActivateSeenArrow();
        ShowExclamation();
    }
   
    //---------------------------GENERAL FUNCTIONS-------------------------------

    public void ReturnToLastState() => SwitchToNextState(lastState);
    public void SetLight(bool active) => lightSource.SetActive(active);
    

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
    

    public List<Waypoint> getWayPoints() => waypoints;


    private void OnDrawGizmosSelected()
    {
        //MAX DISTANCE
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, maxViewDistance);
        
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, minViewDistance);
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackDistance);
        
        if(Charm != null)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(transform.position, Charm.GetInteractDistance());
        }
        
        //VISION CONE
        Vector3 leftBoundary = Quaternion.Euler(0, -viewAngle / 2f, 0) * transform.forward * maxViewDistance;
        Vector3 rightBoundary = Quaternion.Euler(0, viewAngle / 2f, 0) * transform.forward * maxViewDistance;

        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + leftBoundary);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary);
    }

    public Vector3 GetPosition() => transform.position;
    public Transform GetTransform() => transform;
    public void SetVisiblity(bool active) => heart.SetActive(active);
    public Vector3 GetVisionPosition() => transform.position;
    private void OnDestroy()
    {
        pController.RemoveEnemy(this);
        pController.RemoveVisible(this);
    }
}




