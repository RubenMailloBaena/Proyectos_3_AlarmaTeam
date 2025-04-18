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
public class EnemyController : MonoBehaviour, IVisible
{
    private State currentState, nextState, lastState;
    
    private PlayerController pController;
    private EnemySeenHUD eHUD;

    [Header("DEBUG TEXT")]
    [SerializeField] private TextMeshProUGUI debugText;

    [Header("REFERENCES")]
    [SerializeField] public Transform seenExclamationPos;
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
    [HideInInspector] public bool isChasingPlayer;
    [HideInInspector] public bool killingPlayer;
    [HideInInspector] public bool exclamationShown;
    
    
    //ENEMY COMPONENTS
    public EnemyMovement Movement { get; private set; }
    public EnemyRender Renderer { get; private set; }
    public EnemyCharm Charm { get; private set; }
    public EnemyVision Vision { get; private set; }
    public EnemyHear Hear { get; private set; }

    void Awake()
    {
        Movement = GetComponent<EnemyMovement>();
        Renderer = GetComponent<EnemyRender>();
        Charm = GetComponent<EnemyCharm>();
        Vision = GetComponent<EnemyVision>();
        Hear = GetComponent<EnemyHear>();

        Movement.SetMovement(this);
        Renderer.SetRenderer();
        Charm.SetCharm(this);
        Vision.SetVision(this);
        Hear.SetHear(this);
        
        SwitchToNextState(idleState);
    }

    void Start()
    {
        GameManager.GetInstance().GetPlayerController().AddVisible(this);
        pController = GameManager.GetInstance().GetPlayerController();
        eHUD = GameManager.GetInstance().GetEnemySeenHUD();
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
        
        lastState = currentState;
        currentState = nextState;
        currentState?.SetReference(this);
        currentState?.InitializeState();
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
    public void SwitchToLookAtState() => SwitchToNextState(lookAtState);
    public void SwitchToHearState() => SwitchToNextState(hearState);
    public void ReturnToLastState() => SwitchToNextState(lastState);
    public List<Waypoint> getWayPoints() => waypoints;
    private void OnDestroy() => pController.RemoveVisible(this);
    
    //----------------------------VISION FUNCTIONS-----------------------------
    public void SetVisiblity(bool active) => heart.SetActive(active);
    public Vector3 GetVisionPosition() => transform.position;
    
    //----------------------------SEEN HUD FUNCTIONS-----------------------------
    public void ActivateSeenArrow() => eHUD.SetNewArrow(seenExclamationPos, gameObject.GetInstanceID());
    public void UpdateSeenAmount(float amount, float maxAmount) => eHUD.UpdateArrowFillAmount(gameObject.GetInstanceID(), amount, maxAmount);
    public void HideArrow()
    {
        if (lastState == seenState || currentState == seenState)
            eHUD.HideArrow(gameObject.GetInstanceID());
    }
    
    public void ShowExclamation() => eHUD.ShowExclamation(gameObject.GetInstanceID());
    
    public void ActivateExclamation()
    {
        if (exclamationShown) return;
        exclamationShown = true;
        
        if (lastState != seenState)
            ActivateSeenArrow();
        ShowExclamation();
    }
}




