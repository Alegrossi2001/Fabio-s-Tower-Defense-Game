using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehaviourManager : MonoBehaviour
{
    private Transform hq;
    private List<Transform> targetList= new List<Transform>();
    private Dictionary<Enemy, NavMeshAgent> enemyPositions = new Dictionary<Enemy, NavMeshAgent>();
    private List<Enemy> enemiesInTheScene = new List<Enemy>();

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
        StartCoroutine(WaitBeforeTriggeringWave(e.enemies));
    }

    private IEnumerator WaitBeforeTriggeringWave(List<GameObject> enemies)
    {
        yield return new WaitForSeconds(0.5f);
        foreach (GameObject enemy in enemies)
        {
            Debug.Log(enemy + " is an enemy in the scene");
            NavMeshAgent agent = enemy.GetComponent<NavMeshAgent>();
            IEnemyMovement enemyMovement = new EnemyMovement(agent);
            Enemy enemyObject = new Enemy(enemy, enemyMovement, hq);
            enemyObject.InitialiseBehaviour();
            enemyPositions[enemyObject] = agent;
            enemiesInTheScene.Add(enemyObject);
        }

        OnEnemySpawn?.Invoke(this, new OnEnemySpawnEventArgs
        {
            enemies = enemies,
            enemiesObject = enemiesInTheScene,

        });
    }

    private void RemoveEnemyFromScene(object sender, OnEnemySpawnEventArgs e)
    {
        enemiesInTheScene.Remove(e.enemyObject);
        List<GameObject> enemies = new List<GameObject>();
        foreach(Enemy enemy in enemiesInTheScene)
        {
            enemies.Add(enemy.assignedObject);
        }
        OnEnemySpawn?.Invoke(this, new OnEnemySpawnEventArgs
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
                targetList.Add(hq);
            }
        }
        else
        {
            targetList.Add(e.buildingPosition);
        }
        MoveToNearestTarget();
        
    }

    private void RemoveBuildingFromPotentialTargets(object sender, OnBuildingActionEventArgs e)
    {
        if (targetList.Contains(e.buildingPosition))
        {
            targetList.Remove(e.buildingPosition);
            MoveToNearestTarget();
        }
        else
        {
            Debug.LogError("Error, the list does not contain that building");
        }
        GameManager.Instance.CheckForGameOver(hq);
    }

    private void MoveToNearestTarget()
    {
        foreach (Enemy enemy in enemiesInTheScene)
        {
            enemy.MoveToNewPosition(targetList, enemy.GetCurrentPosition());
        }
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
