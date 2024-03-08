using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawnManager : MonoBehaviour
{
    [SerializeField] private LayerMask layerNumber;
    [SerializeField] private int numberOfSpawns; //temp value
    [SerializeField] private GameObject parentObject;
    private EnemyObjectPooling pool;
   
    //Move this to a function

    private void Awake()
    {
        Building.OnBuildingAction += WaveStarted;
        Vector3 origin = this.transform.position;
        Vector3 direction = this.transform.forward;
        pool = new EnemyObjectPooling(origin, direction, layerNumber, parentObject);
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
        pool.SpawnEnemyWave(numberOfSpawns);
    }
}
