using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    private NavMeshAgent meshAgent;
    private EnemyController eController;
    private PlayerController pController;
    
    private List<Waypoint> waypoints;
    private int waypointIndex;
    [SerializeField] private float minAngleDiffToRotate = 5f;    
    [SerializeField] private float minDistanceToArrive = 0.3f;
    
    [HideInInspector] public Vector3 enemyPosBeforeMoving;

    public void SetMovement(EnemyController enemyController)
    {
        eController = enemyController;
        waypoints = eController.getWayPoints();
        pController = GameManager.GetInstance().GetPlayerController();
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
    
    public void IncrementIndex()
    {
        waypointIndex++;
        if (waypointIndex >= waypoints.Count)
            waypointIndex = 0;
    }

    //MESH AGENT CHANGES
    public void SetAgentSpeed(float speed) => meshAgent.speed = speed;

    public void StopAgent() => meshAgent.ResetPath();
    
    public Vector3 GetEnemyVelocity() => meshAgent.velocity;
    
    public void ManualRotation(bool active) => meshAgent.updateRotation = !active;

    public Vector3 GoToDestination(Vector3 target)
    {
        meshAgent.SetDestination(target);
        return target;
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
    
    public Vector3 GoToLever()
    {
        meshAgent.SetDestination(eController.Charm.GetLeverPosition());
        return eController.Charm.GetLeverPosition();
    }

    public Vector3 GoToSoundSource()
    {
        meshAgent.SetDestination(eController.Hear.soundPos);
        return eController.Hear.soundPos;
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
