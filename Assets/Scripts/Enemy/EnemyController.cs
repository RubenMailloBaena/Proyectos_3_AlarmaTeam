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

    [Header("ENEMY OPTIONS")] 
    public bool isIdleEnemy;
    public EnemyType enemyType;

    [Header("WAY POINTS")] 
    [SerializeField] private List<Waypoint> waypoints;
    private int waypointIndex;
    private float minDistanceToArrive = 0.1f;
    
    //VARIABLES
    [HideInInspector] public bool soundWasAnObject;
    [HideInInspector] public Vector3 soundPos;

    void Awake()
    {
        meshAgent = GetComponent<NavMeshAgent>();
        SwitchToNextState(idleState);
    }

    void Start()
    {
        pController = GameManager.GetInstance().GetPlayerController();
    }

    void Update()
    {
        RunStateMachine();
    }

    private void RunStateMachine()
    {
        nextState = currentState?.RunCurrentState();

        if (nextState != null && nextState != currentState)
            SwitchToNextState(nextState);
    }

    public void SwitchToNextState(State nextState)
    {
        //DEBUG ONLY
        debugText.text = nextState.name;
        //
        lastState = currentState;
        currentState = nextState;
        currentState?.SetReference(this);
        currentState?.InitializeState();
    }
    //----------------------------ICanHear FUNCTIONS-----------------------------
    
    public void HeardSound(Vector3 soundPoint, bool isObject)
    {
        soundWasAnObject = isObject;
        soundPos = soundPoint;
        soundPos.y = transform.position.y;
        SwitchToNextState(hearState);
    }
    
    //---------------------------GENERAL FUNCTIONS-------------------------------
    
    public void SetAgentSpeed(float speed) => meshAgent.speed = speed;
    public void SetAgentDestination(Vector3 position) => meshAgent.SetDestination(position);
    public void ReturnToLastState() => SwitchToNextState(lastState);
    public void StopAgent() => meshAgent.ResetPath();

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
        if (angleDiff < 1f)
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
