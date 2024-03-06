using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : IEnemyMovement
{
    private NavMeshAgent agent;
    public Vector3 currentPosition;

    public EnemyMovement(NavMeshAgent agent)
    {
        this.agent = agent;
    }

    public void MoveToNewPosition(Vector3 position)
    {
        agent.SetDestination(position);
    }

    public void UpdateCurrentPosition(Vector3 currentPosition)
    {
        this.currentPosition= currentPosition;
    }

    public void UpdateTargetToClosestBuilding(List<Transform> targets, Vector3 currentPosition)
    {
        Vector3 newPosition = FindClosestObject(targets, currentPosition).position;
        agent.SetDestination(newPosition);
    }

    private Transform FindClosestObject(List<Transform> targets, Vector3 currentPosition)
    {
        return Utilities.GetClosestObject(targets, currentPosition);
    }

    public Vector3 GetCurrentPosition()
    {
        return currentPosition;
    }
}
