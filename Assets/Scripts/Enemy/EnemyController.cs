using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyType{
    Priest,
    Knight
}
public class EnemyController : MonoBehaviour
{
    private State currentState, nextState;
    
    private NavMeshAgent meshAgent;
    private PlayerController pController;

    [Header("INITIAL STATE")]
    [SerializeField] private IdleState idleState;

    [Header("ENEMY OPTIONS")] 
    public bool isIdleEnemy;
    public EnemyType enemyType;

    [Header("WAY POINTS")] 
    [SerializeField] private List<Waypoint> waypoints;
    private int waypointIndex;
    private float minDistanceToArrive = 0.1f;

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
        print("Entering: " + nextState.name);
        currentState = nextState;
        currentState?.SetReference(this);
        currentState?.InitializeState();
    }
    
    //---------------------------GENERAL FUNCTIONS-------------------------------
    public float GetWaitTime()
    {
        if (waypoints.Count == 0) 
            return 0.0f;
        return waypoints[waypointIndex].waitTime;
    }
    
    public void SetAgentSpeed(float speed) => meshAgent.speed = speed;
    public void SetAgentDestination(Vector3 position) => meshAgent.SetDestination(position);
    
    public Vector3 GoToWaypoint()
    {
        Vector3 pos = waypoints[waypointIndex].Position();
        meshAgent.SetDestination(pos);
        return pos;
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
}


[Serializable]
public class Waypoint
{
    public float waitTime = 0.0f;
    public Transform waypoint;
    public Vector3 Position() => waypoint.position;
}
