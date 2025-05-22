using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public enum EnemyType{
    Priest,
    Knight
}
public class EnemyController : MonoBehaviour, IVisible
{
    private State currentState, nextState, lastState;
    
    private PlayerController pController;
    private EnemySeenHUD eHUD;
    private CapsuleCollider enemyCollider;

    [Header("DEBUG TEXT")]
    [SerializeField] private TextMeshProUGUI debugText;

    [Header("REFERENCES")]
    [SerializeField] public Transform seenExclamationPos;
    [SerializeField] private EnemyCheckpointAdapter checkpointAdapter;

    [Header("STATES")]
    [SerializeField] private IdleState idleState;
    [SerializeField] private HearState hearState;
    [SerializeField] private LookAtState lookAtState;
    [SerializeField] private SeenState seenState;
    [SerializeField] private ChaseState chaseState;
    [SerializeField] private AttackState attackState;
    [SerializeField] private CharmState charmState;
    [SerializeField] private CheckState checkState;
    [SerializeField] private DieState dieState;

    [Header("ENEMY OPTIONS")] 
    public bool isIdleEnemy;
    public EnemyType enemyType;

    [Header("WAY POINTS")] 
    [SerializeField] private List<Waypoint> waypoints;
    
    //VARIABLES
    [HideInInspector] public bool isChasingPlayer;
    [HideInInspector] public bool killingPlayer;
    [HideInInspector] public bool exclamationShown;
    [HideInInspector] public bool enemyIsDead;
    
    //CHECKPOINTS
    private Coroutine killPlayerCoroutine;
    
    //ENEMY COMPONENTS
    private EnemyMovement Movement { get; set; }
    private EnemyRender Renderer { get; set; }
    private EnemyCharm Charm { get; set; }
    private EnemyVision Vision { get; set; }
    private EnemyHear Hear { get; set; }
    private EnemyBackstab Backstab { get; set; }
    private EnemyRestart Restart { get; set; }
    private EnemyAnimations Animations { get; set; }

    void Awake()
    {
        Movement = GetComponent<EnemyMovement>();
        Renderer = GetComponent<EnemyRender>();
        Charm = GetComponent<EnemyCharm>();
        Vision = GetComponent<EnemyVision>();
        Hear = GetComponent<EnemyHear>();
        Backstab = GetComponent<EnemyBackstab>();
        Restart = GetComponent<EnemyRestart>();
        Animations = GetComponent<EnemyAnimations>();

        Movement.SetMovement(this);
        Charm.SetCharm(this);
        Vision.SetVision(this);
        Hear.SetHear(this);
        Backstab.SetBackstab(this);
        Restart.SetRestart(this);
        
        SwitchToNextState(idleState);

        enemyCollider = GetComponent<CapsuleCollider>();
    }

    void Start()
    {
        pController = GameManager.GetInstance().GetPlayerController();
        pController.AddVisible(this);
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
        if (killingPlayer || enemyIsDead) return;
        
        //DEBUG ONLY
        //Debug.LogError(nextState.name.ToUpper());
        debugText.text = nextState.name.ToUpper();
        
        lastState = currentState;
        currentState = nextState;
        currentState?.SetReference(this);
        currentState?.InitializeState();
    }

    //---------------------------GENERAL FUNCTIONS-------------------------------

    #region General Functions
    public State GetCurrentState() => currentState;
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
    public void SwitchToDieState() => SwitchToNextState(dieState);
    public void ReturnToLastState() => SwitchToNextState(lastState);
    public List<Waypoint> GetWayPoints() => waypoints;
    public PlayerController GetPlayerController() => pController;
    public void SetCollider(bool active) => enemyCollider.enabled = active;
    public void EnemyDead()
    {
        enemyIsDead = true;
        checkpointAdapter.EnemyDead();
        if (lastState == charmState)
            SetCharmLockedVisual(false);
    }

    public void EnemyRespawn()
    {
        checkpointAdapter.EnemyRespawn();
    }
    public float GetCurrentWaitTime()
    {
        if (currentState == idleState)
            return idleState.GetCurrentWaitTime();
        return 0.0f;
    }

    public void AddCurrentWaitTime(float time)
    {
        if (currentState == idleState)
            idleState.AddCurrentWaitTime(time);
    }

    private void OnDestroy()
    {
        if(pController != null)
            pController.RemoveVisible(this);
        if(!enemyIsDead) GameManager.GetInstance().RemoveEnemieAlive();
    }

    #endregion
    
    //----------------------------ENEMY VISION FUNCTIONS-----------------------------

    #region Animation Functions
    
    public void SetAnimation(AnimationType animation, bool setBool) => Animations.SetAnimation(animation, setBool);
    #endregion
    
    //----------------------------ENEMY VISION FUNCTIONS-----------------------------

    #region Vision Functions
    public bool IsPointInVision(Vector3 target) => Vision.IsPointInVision(target);
    public float GetMaxViewDis() => Vision.GetMaxViewDis();
    public float GetMinViewDis() => Vision.GetMinViewDis();
    public float GetExitAttackRange() => Vision.GetExitAttackRange();
    public bool IsPlayerInVision() => Vision.IsPlayerInVision;
    public bool SetIgnorePlayerInMinVision(bool b) => Vision.IgnorePlayerInMinVision = b;
    #endregion
    
    //----------------------------IVISION FUNCTIONS-----------------------------
    
    #region IVision

    public void SetVisiblity(bool active) => SetWeakSpot(active);
    public Vector3 GetVisionPosition() => transform.position;
    #endregion
    
    //----------------------------HEAR FUNCTIONS-----------------------------

    #region Hear Functions
    public Vector3 SoundPos() => Hear.SoundPos;
    public Vector3 SetSoundPos(Vector3 soundPos) => Hear.SoundPos = soundPos;
    public bool SoundWasAnObject() => Hear.SoundWasAnObject;
    public bool SetSoundWasAnObject(bool b) => Hear.SoundWasAnObject = b;
    public bool SetInPlayerHearState(bool b) => Hear.InPlayerHearState = b;
    #endregion
    
    //----------------------------CHARM FUNCTIONS-----------------------------

    #region Charm Functions
    public Vector3 GetLeverPosition() => Charm.GetLeverPosition();
    public void InteractLever() => Charm.InteractLever();
    public void SetCharmLockedVisual(bool active) => Charm.SetLockedVisual(active);
    #endregion
    
    //----------------------------RENDER FUNCTIONS-----------------------------

    #region Render Functions
    public void ChangeOutline(bool active) => Renderer.ChangeOutline(active);
    public void SetLocked(bool locked) => Renderer.SetLocked(locked);
    public void SetRenderActive(bool active) => Renderer.SetRenderActive(active);
    public void SetLight(bool active) => Renderer.SetLight(active);
    public void SetWeakSpot(bool active) => Backstab.SetWeakSpot(active);
    public void KillAnimationHeartController(bool active) => Backstab.KillAnimationHeartController(active);
    #endregion
    
    //----------------------------MOVEMENT FUNCTIONS-----------------------------

    #region Movement Functions
    public float GetWaitTime() => Movement.GetWaitTime();
    public Vector3 GoToWaypoint() => Movement.GoToWaypoint();
    public Vector3 GetLookDirection() => Movement.GetLookDirection();
    public bool ArrivedToPosition(Vector3 position) => Movement.ArrivedToPosition(position);
    public bool ArrivedToLever(Vector3 position) => Movement.ArrivedToLever(position);
    public void IncrementIndex() => Movement.IncrementIndex();
    public void StopAgent() => Movement.StopAgent();
    public Vector3 GetEnemyVelocity() => Movement.GetEnemyVelocity();
    public void ManualRotation(bool active) => Movement.ManualRotation(active);
    public void WarpAgent(Vector3 position) => Movement.WarpAgent(position);
    public Vector3 GoToLever() => Movement.GoToDestination(Charm.GetLeverPosition());
    public Vector3 GoToSoundSource() => Movement.GoToDestination(Hear.SoundPos);
    public Vector3 GoToPreviousPosition() => Movement.GoToPreviousPosition();
    public Vector3 GoToPlayerPosition() => Movement.GoToDestination(pController.GetPlayerPosition());
    public float GetPathLength(Vector3 target) => Movement.GetPathLength(target);
    public bool RotateEnemy(Vector3 lookDir, float rotationSpeed) => Movement.RotateEnemy(lookDir, rotationSpeed);
    public void SetPositionBeforeMoving() => Movement.SetPositionBeforeMoving();
    public Vector3 SetEnemyPosBeforeMoving(Vector3 pos) => Movement.EnemyPosBeforeMoving = pos;
    public void RestartIndex() => Movement.RestartIndex();
    public int GetIndex() => Movement.GetIndex();
    public void SetIndex(int indx) => Movement.SetIndex(indx);
    public void SetAgentSpeed(float speed)
    {
        if (isChasingPlayer) return;
        Movement.SetAgentSpeed(speed);
    }
    #endregion
    
    //----------------------------SEEN HUD FUNCTIONS-----------------------------

    #region See HUD Functions
    public void ActivateSeenArrow() => eHUD.SetNewArrow(seenExclamationPos, gameObject.GetInstanceID());
    public void UpdateSeenAmount(float amount, float maxAmount) => eHUD.UpdateArrowFillAmount(gameObject.GetInstanceID(), amount, maxAmount);
    public void ShowExclamation() => eHUD.ShowExclamation(gameObject.GetInstanceID());
    public void HideArrow()
    {
        if (lastState == seenState || currentState == seenState)
            eHUD.HideArrow(gameObject.GetInstanceID());
    }
    
    public void ActivateExclamation()
    {
        if (exclamationShown) return;
        exclamationShown = true;
        
        if (lastState != seenState)
            ActivateSeenArrow();
        ShowExclamation();
    }
    #endregion
    
    //----------------------------PLAYER TP WHILE CHASING FUNCTIONS-----------------------------

    #region PlayerTP on Chasing or Attacking
    private void SwapToCheckIfTP()
    {
        if (IsChasing() || IsAttacking())
            SwitchToNextState(checkState);
    }
    
    private void OnEnable()
    {
        GameManager.GetInstance().GetPlayerController().OnPlayerTP += SwapToCheckIfTP;
    }

    private void OnDisable()
    {
        GameManager.GetInstance().GetPlayerController().OnPlayerTP -= SwapToCheckIfTP;
    }
    #endregion

    //-------------------KILL PLAYER---------------------------

    public void KillPlayer()
    {
        killPlayerCoroutine = StartCoroutine(KillPlayerAnimation());
    }

    public void StopKillCoroutine()
    {
        killingPlayer = false;

        if(killPlayerCoroutine != null)
            StopCoroutine(killPlayerCoroutine);
    }
    
    private IEnumerator KillPlayerAnimation()
    {
        Transform playerTrans = pController.GetPlayerTransform();
        killingPlayer = true;
        StopAgent();

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

        yield return null;
    }
}