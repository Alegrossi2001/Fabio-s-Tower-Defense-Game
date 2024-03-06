using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public interface IEnemyMovement
{
    public void MoveToNewPosition(Vector3 position);
    public void UpdateTargetToClosestBuilding(List<Transform> buildings, Vector3 currentPosition);
    public void UpdateCurrentPosition(Vector3 currentPosition);
    public Vector3 GetCurrentPosition();
}
