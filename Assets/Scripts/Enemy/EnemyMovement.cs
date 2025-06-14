using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    private NavMeshAgent meshAgent;
    private EnemyController eController;
    
    private List<Waypoint> waypoints;
    private int waypointIndex;
    [SerializeField] private float minAngleDiffToRotate = 5f;    
    [SerializeField] private float minDistanceToArrive = 0.3f;
    [SerializeField] private float minDistanceToArriveToLever = 1f;
    
    private Vector3 enemyPosBeforeMoving;

    public void SetMovement(EnemyController enemyController)
    {
        eController = enemyController;
        waypoints = eController.GetWayPoints();
        meshAgent = GetComponent<NavMeshAgent>();
        enemyPosBeforeMoving = Vector3.zero;
    }

    //WAYPOINTS
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
    
    public bool ArrivedToPosition(Vector3 position)
    {
        return Vector3.Distance(transform.position, position) <= minDistanceToArrive;
    }
    
    public bool ArrivedToLever(Vector3 position)
    {
        return Vector3.Distance(transform.position, position) <= minDistanceToArriveToLever;
    }
    
    public void IncrementIndex()
    {
        waypointIndex++;
        if (waypointIndex >= waypoints.Count)
            waypointIndex = 0;
    }

    public void RestartIndex()
    {
        waypointIndex = 0;
    }

    public int GetIndex() => waypointIndex;
    public void SetIndex(int index) => waypointIndex = index;

    //MESH AGENT CHANGES
    public void SetAgentSpeed(float speed) => meshAgent.speed = speed;

    public void StopAgent() => meshAgent.ResetPath();
    
    public Vector3 GetEnemyVelocity() => meshAgent.velocity;
    
    public void ManualRotation(bool active) => meshAgent.updateRotation = !active;

    public void WarpAgent(Vector3 position) => meshAgent.Warp(position);
    
    public Vector3 GoToDestination(Vector3 target)
    {
        meshAgent.SetDestination(target);
        return target;
    }
    
    public Vector3 GoToPreviousPosition()
    {
        meshAgent.SetDestination(enemyPosBeforeMoving);
        return enemyPosBeforeMoving;
    }

    public float GetPathLength(Vector3 target)
    {
        NavMeshPath path = new NavMeshPath();

        if (meshAgent.CalculatePath(target, path) && path.status == NavMeshPathStatus.PathComplete) //Exists a path
        {
            float pathLength = 0.0f;
            for (int i = 1; i < path.corners.Length; i++)
            {
                pathLength += Vector3.Distance(path.corners[i - 1], path.corners[i]);
            }
            //Debug.LogWarning("Distance to sound: " + pathLength);
            return pathLength;
        }
        return -1f;
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
    
    public void SetPositionBeforeMoving()
    {
        if (enemyPosBeforeMoving == Vector3.zero)
            enemyPosBeforeMoving = transform.position;
    }

    public Vector3 EnemyPosBeforeMoving
    {
        set => enemyPosBeforeMoving = value;
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
