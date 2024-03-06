using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehaviourManager : MonoBehaviour
{
    private Transform hq;
    private List<Transform> targetList= new List<Transform>();
    private Dictionary<Enemy, NavMeshAgent> enemyPositions = new Dictionary<Enemy, NavMeshAgent>();
    private List<Enemy> enemiesInTheScene = new List<Enemy>();

    public static EventHandler<OnEnemySpawnEventArgs> OnEnemyDeath;
    public static EventHandler<OnEnemySpawnEventArgs> OnEnemySpawn;

    void Awake()
    {
        Building.OnBuildingAction += AddBuildingToPotentialTargets;
        BuildingHealth.onBuildingDestruction += RemoveBuildingFromPotentialTargets;
        EnemySpawnManager.OnEnemySpawn += SpawnEnemyWithBehaviour;
        Enemy.OnEnemyDeath += RemoveEnemyFromScene;
    }

    private void SpawnEnemyWithBehaviour(object sender, OnEnemySpawnEventArgs e)
    {
        foreach(GameObject enemy in e.enemies)
        {
            Debug.Log(enemy + " is an enemy in the scene");
            NavMeshAgent agent = enemy.GetComponent<NavMeshAgent>();
            IEnemyMovement enemyMovement = new EnemyMovement(agent);
            Enemy enemyObject = new Enemy(enemy, enemyMovement, hq);
            enemyObject.InitialiseBehaviour();
            enemyPositions[enemyObject] = agent;      
            enemiesInTheScene.Add(enemyObject);
        }

        //questo codice non viene raggiunto

        OnEnemySpawn?.Invoke(this, new OnEnemySpawnEventArgs
        {
            enemies = e.enemies,
            enemiesObject = enemiesInTheScene,
            
        }) ;
    }

    private void RemoveEnemyFromScene(object sender, OnEnemySpawnEventArgs e)
    {
        enemiesInTheScene.Remove(e.enemyObject);
        List<GameObject> enemies = new List<GameObject>();
        foreach(Enemy enemy in enemiesInTheScene)
        {
            enemies.Add(enemy.assignedObject);
        }
        OnEnemyDeath?.Invoke(this, new OnEnemySpawnEventArgs
        {
            enemies = enemies,
            enemiesObject = enemiesInTheScene
        });
    }

    private void AddBuildingToPotentialTargets(object sender, OnBuildingActionEventArgs e)
    {
        if(hq == null)
        {
            if(e.isHQ == true)
            {
                hq = e.buildingPosition;
            }
        }
        targetList.Add(e.buildingPosition);
        MoveToNearestTarget();

    }

    private void RemoveBuildingFromPotentialTargets(object sender, OnBuildingActionEventArgs e)
    {
        if (targetList.Contains(e.buildingPosition))
        {
            targetList.Remove(e.buildingPosition);
            Debug.Log("Building Destroyed, count is " + e.buildingPosition);
            MoveToNearestTarget();
        }
        else
        {
            Debug.Log("Error, the list does not contain that building");
        }
        
    }

    private void MoveToNearestTarget()
    {
        foreach (Enemy enemy in enemiesInTheScene)
        {
            enemy.MoveToNewPosition(targetList, enemy.GetCurrentPosition());
        }
    }

    private bool CheckForHQOrGameOver()
    {
        return hq!= null;
    }

    private void Update()
    {
        foreach(Enemy enemy in enemyPositions.Keys)
        {
            try
            {
                Vector3 enemyPosition = enemyPositions[enemy].nextPosition;
                enemy.UpdateCurrentPosition(enemyPosition);
            }
            catch(MissingReferenceException e)
            {
                enemiesInTheScene.Remove(enemy);
            }
        }
    }



}
