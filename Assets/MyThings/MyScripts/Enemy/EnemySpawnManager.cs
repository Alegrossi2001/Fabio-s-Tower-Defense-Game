using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawnManager : MonoBehaviour
{
    [SerializeField] private LayerMask layerNumber;
    public static EventHandler<OnEnemySpawnEventArgs> OnEnemySpawn;
    [SerializeField] private int numberOfSpawns; //temp value
    //Move this to a function

    private void Awake()
    {
        Building.OnBuildingAction += WaveStarted;
    }

    private void WaveStarted(object sender, OnBuildingActionEventArgs e)
    {
        if(e.isHQ == true)
        {
            TriggerNewWave();
            Building.OnBuildingAction -= WaveStarted;
        }
        else
        {
            Debug.LogError("HQ hasn't spawned correctly");
        }
    }

    private void TriggerNewWave()
    {
        EnemySpawnHandler handler = new EnemySpawnHandler();
        Vector3 origin = this.transform.position;
        Vector3 direction = this.transform.forward;
        List<Transform> spawnPoints = handler.GetEnemySpawnPoints(origin, direction, layerNumber);
        List<GameObject> enemiesForThisWave = handler.SpawnEnemyWave(spawnPoints, numberOfSpawns);
        OnEnemySpawn?.Invoke(this, new OnEnemySpawnEventArgs
        {
            enemies = enemiesForThisWave
        });
    }
}
