using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingHealth
{
    private int health;
    private GameObject buildingAssigned;
    public static EventHandler<OnBuildingActionEventArgs> onBuildingDestruction;
    private Building assignedBuilding;

    public BuildingHealth(Building assignedBuilding, GameObject building, int startingHealth)
    {
        this.assignedBuilding = assignedBuilding;  
        health = startingHealth;
        buildingAssigned = building;
    }
   

    public void ReduceHealth(int damage)
    {
        health -= damage;
        if(health <= 0)
        {
            DestroyBuilding(buildingAssigned);
        }
    }

    private void DestroyBuilding(GameObject building)
    {
        //add fancy animation here
        onBuildingDestruction?.Invoke(this, new OnBuildingActionEventArgs
        {
            building = assignedBuilding,
            buildingPosition = building.transform
        });
        GameObject.Destroy(buildingAssigned);
    }
}
