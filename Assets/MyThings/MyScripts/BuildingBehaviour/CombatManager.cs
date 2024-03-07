using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;
using Unity.VisualScripting;

public class CombatManager : MonoBehaviour
{
    private List<Building> buildingsInTheScene = new List<Building>();
    private List<Enemy> enemiesInTheScene = new List<Enemy>();

    private void Awake()
    {
        Building.OnBuildingAction += AddBuildingToDefensiveTargets;
        BuildingHealth.onBuildingDestruction += RemoveBuildingFromDefensiveTargets;
        EnemyBehaviourManager.OnEnemySpawn += HandleNewEnemyList;
        EnemyBehaviourManager.OnEnemySpawn+= HandleNewEnemyList;
    }

    //move into an event driven function;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            foreach (Building building in buildingsInTheScene)
            {
               TriggerAttack(building);
            }
        }
    }

    private void AddBuildingToDefensiveTargets(object sender, OnBuildingActionEventArgs e)
    {
        try
        {
            buildingsInTheScene.Add(e.building);
        }
        catch(NullReferenceException error)
        {
            Debug.Log(error);
        }
    }

    private void RemoveBuildingFromDefensiveTargets(object sender, OnBuildingActionEventArgs e)
    {
        try
        {
            buildingsInTheScene.Remove(e.building);
        }
        catch(NullReferenceException error)
        {
            Debug.Log(error);
        }
    }

    private void HandleNewEnemyList(object sender, OnEnemySpawnEventArgs e)
    {
        enemiesInTheScene = e.enemiesObject;
        foreach(Building building in buildingsInTheScene)
        {
            TriggerAttack(building);
        }
    }
    private void TriggerAttack(Building building)
    {
        try
        {
            List<Vector3> vector3List = enemiesInTheScene.Select(go => go.GetCurrentPosition()).ToList();
            building.SetTargets(vector3List);
            if (building.CalculateClosestTarget() == true)
            {
                building.ShootBulletAtClosestEnemy(building.GetClosestEnemyTarget(enemiesInTheScene));
            }

        }
        catch(NullReferenceException e)
        {
            Debug.LogError("There are no buildings in the scene yet!");
        }
    }


}
