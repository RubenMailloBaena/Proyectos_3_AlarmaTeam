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

    [Header("WAY POINTS")] 
    [SerializeField] private List<Waypoint> waypoints;
    
    //VARIABLES
    [HideInInspector] public bool soundWasAnObject = true;
    [HideInInspector] public bool inPlayerHearState = true;
    [HideInInspector] public bool isChasingPlayer;
    [HideInInspector] public bool killingPlayer;
    [HideInInspector] public bool exclamationShown;
    [HideInInspector] public Vector3 soundPos;
    [HideInInspector] public Vector3 enemyPosBeforeMoving;
    
    //ENEMY COMPONENTS
    public EnemyMovement Movement { get; private set; }
    public EnemyRender Renderer { get; private set; }
    public EnemyCharm Charm { get; private set; }
    public EnemyVision Vision { get; private set; }

    void Awake()
    {
        Movement = GetComponent<EnemyMovement>();
        Renderer = GetComponent<EnemyRender>();
        Charm = GetComponent<EnemyCharm>();
        Vision = GetComponent<EnemyVision>();

        Movement.SetMovement(this);
        Renderer.SetRenderer();
        Charm.SetCharm(this);
        Vision.SetVision(this);
        
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
    public bool IsCharmed() => currentState == charmState;
    public bool IsAttacking() => currentState == attackState;
    public bool IsChasing() => currentState == chaseState;
    public bool InSeenState() => currentState == seenState;
    public void SwitchToCharmState() => SwitchToNextState(charmState);
    public void SwitchToAttackState() => SwitchToNextState(attackState);
    public void SwitchToChaseState() => SwitchToNextState(chaseState);
    public void SwitchToSeenState() => SwitchToNextState(seenState);
    public void ReturnToLastState() => SwitchToNextState(lastState);
    public List<Waypoint> getWayPoints() => waypoints;
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




