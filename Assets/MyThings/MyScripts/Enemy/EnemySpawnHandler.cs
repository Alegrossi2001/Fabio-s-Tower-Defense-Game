using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnHandler
{
    private float radius = 10000;
    private float maxDistance = 10000;
    private EnemyListSO enemyList;

    public EnemySpawnHandler()
    {
        this.enemyList = Resources.Load<EnemyListSO>(typeof(EnemyListSO).Name);
    }

    public List<Transform> GetEnemySpawnPoints(Vector3 origin, Vector3 direction, LayerMask layerNumber)
    {
        RaycastHit[] hits = Physics.SphereCastAll(origin, radius, direction, maxDistance, layerNumber);
        List<Transform> transforms = new List<Transform>();
        foreach(RaycastHit hit in hits)
        {
            transforms.Add(hit.transform);
        }
        return transforms;
    }

    public List<EnemySpawnResult> SpawnEnemyWave(List<Transform> spawnPositions, int enemiesToSpawn)
    {
        List<Transform> enemySpawnPositions = Utilities.GetRandomListOfObjects(spawnPositions, enemiesToSpawn, true);
        List<EnemySpawnResult> enemies = new List<EnemySpawnResult>();
        foreach(Transform spawn in enemySpawnPositions)
        {
            Vector3 spawnPoint = spawn.position + new Vector3(0, 1.5f, 0); //offset;
            int randomIndex = new System.Random().Next(0, enemyList.EnemyTypes.Count);
            EnemySO randomEnemy = enemyList.EnemyTypes[randomIndex];
            GameObject enemyToSpawn = UnityEngine.Object.Instantiate(randomEnemy.enemyPrefab, spawnPoint, Quaternion.identity);
            enemies.Add(new EnemySpawnResult
            {
                enemy = enemyToSpawn,
                spawnPoint = spawn
            });
        }
        return enemies;
    }

     
    
}
