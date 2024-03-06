using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingCollisionHandler : MonoBehaviour
{
    private Building buildingAssigned;

    public void AssignBuilding(Building building)
    {
        buildingAssigned = building;
    }

    public void RemoveHealth(int health)
    {
        if (buildingAssigned != null)
        {
            buildingAssigned.healthManager.ReduceHealth(health);
        }
    }
}
