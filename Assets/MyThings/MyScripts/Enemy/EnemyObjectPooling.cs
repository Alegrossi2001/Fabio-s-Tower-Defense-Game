using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EnemyObjectPooling 
{
    private int totalNumberOfEnemies = 100;
    private Vector3 origin;
    private Vector3 direction;
    private LayerMask layerNumber;
    private List<EnemySpawnResult> totalEnemies = new List<EnemySpawnResult>();
    public static EnemyObjectPooling instance = null;
    
    public static EventHandler<OnEnemySpawnEventArgs> OnEnemySpawn;
    private GameObject parentObject;

    public EnemyObjectPooling(Vector3 origin, Vector3 direction, LayerMask layerNumber, GameObject parentObject)
    {
        this.origin = origin;
        this.layerNumber = layerNumber;
        this.direction = direction;
        this.parentObject = parentObject;
        SpawnTotalNumberOfEnemies(this.totalNumberOfEnemies);
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogError("There are more than one instances of the EnemyObjectPooling Class! This is not a singleton!");
        }
    }

    private void SpawnTotalNumberOfEnemies(int totalNumberOfEnemies)
    {
        EnemySpawnHandler handler = new EnemySpawnHandler();
        List<Transform> spawnPoints = handler.GetEnemySpawnPoints(origin, direction, layerNumber);
        List<EnemySpawnResult> totalEnemies = handler.SpawnEnemyWave(spawnPoints, totalNumberOfEnemies);
        List<GameObject> enemies = totalEnemies.Select(spawnedObj => spawnedObj.enemy).ToList();
        foreach (GameObject enemy in enemies)
        {
            enemy.transform.SetParent(parentObject.transform);
            enemy.SetActive(false);
        }
        this.totalEnemies= totalEnemies;
    }

    public void SpawnEnemyWave(int enemyCount)
    {
        try
        {
            System.Random rand = new System.Random();
            List<GameObject> randomEnemies = new List<GameObject>();
            List<GameObject> Enemies = totalEnemies.Select(spawnedObj => spawnedObj.enemy).ToList();
            randomEnemies = Utilities.GetRandomListOfObjects(Enemies, enemyCount, false);
            foreach(GameObject enemy in randomEnemies)
            {
                enemy.SetActive(true);
            }
            OnEnemySpawn?.Invoke(this, new OnEnemySpawnEventArgs
            {
                enemies = randomEnemies
            });

        }
        catch(ArgumentOutOfRangeException e)
        {
            Debug.LogError("You requested more enemies than there are present on the map!");
        }
    }

    public void ReturnEnemyToOriginalSpawnPoint(GameObject enemy)
    {
        EnemySpawnResult result = totalEnemies.FirstOrDefault(obj => obj.enemy == enemy);
        Transform enemySpawnPoint = result.spawnPoint;
        enemy.transform.position = enemySpawnPoint.position;
        enemy.SetActive(false);
    }


    
}
