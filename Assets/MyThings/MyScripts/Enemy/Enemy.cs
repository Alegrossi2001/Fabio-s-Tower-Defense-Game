using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy
{
    private IEnemyMovement movement;
    private Transform startingGoal;
    public GameObject assignedObject { get; private set; }
    public static EventHandler<OnEnemySpawnEventArgs> OnEnemyDeath;

    public Enemy(GameObject assignedObject, IEnemyMovement movement, Transform startingGoal)
    {
        this.assignedObject = assignedObject;
        this.movement = movement;
        this.startingGoal = startingGoal;
        this.assignedObject.GetComponent<EnemyCollisionHandler>().AssignEnemy(this);
    }

    public void InitialiseBehaviour()
    {
        MoveToTarget();
    }

    public void MoveToTarget()
    {
        movement.MoveToNewPosition(startingGoal.position);
    }

    public void MoveToNewPosition(List<Transform> buildingList, Vector3 currentPosition)
    {
        movement.UpdateTargetToClosestBuilding(buildingList, currentPosition);
    }

    public void UpdateCurrentPosition(Vector3 position)
    {
        movement.UpdateCurrentPosition(position);
    }


    public Vector3 GetCurrentPosition()
    {
        return movement.GetCurrentPosition();
    }

    public void EnemyIsDead()
    {
        OnEnemyDeath?.Invoke(this, new OnEnemySpawnEventArgs
        {
            enemyObject = this,
            enemy = this.assignedObject
        });
        EnemyObjectPooling.instance.ReturnEnemyToOriginalSpawnPoint(assignedObject);
        
    }

}
