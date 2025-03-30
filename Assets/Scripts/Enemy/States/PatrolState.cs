using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PatrolState : State
{
    [Header("PATROL")]
    public NavMeshAgent meshAgent;
    public float patrolSpeed = 3f;
    public Transform[] patrolWayPoints;
    private int lastWayPoint = 0;

    [Range(0f, 1f)] public float probToIdle = 0.3f; //Quan arribi a un waypoint probabilitat de passar a IDLE

    [Header("STATES")]
    public IdleState idleState;

    public override void InitializeState()
    {
        Debug.Log("Enter Patrol");
        meshAgent.speed = patrolSpeed;
        meshAgent.SetDestination(patrolWayPoints[lastWayPoint].position);
    }

    public override State RunCurrentState()
    {
        /*
        if (alertState.CanHearPlayer())
            return alertState;
        */

        if (ArrivedToTarget())
        {
            IncrementIndex();
            meshAgent.SetDestination(patrolWayPoints[lastWayPoint].position);

            if (Random.value <= probToIdle)
                return idleState;
        }

        return this;
    }


    private bool ArrivedToTarget()
    {
        return Vector3.Distance(transform.position, patrolWayPoints[lastWayPoint].position) <= 0.5f;
    }

    private void IncrementIndex()
    {
        lastWayPoint++;
        if (lastWayPoint >= patrolWayPoints.Length)
            lastWayPoint = 0;
    }
}
