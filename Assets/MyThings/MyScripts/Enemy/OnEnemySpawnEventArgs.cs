using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnEnemySpawnEventArgs : EventArgs
{
    public List<GameObject>? enemies;
    public List<Enemy>? enemiesObject;
    public GameObject? enemy;
    public Enemy? enemyObject;
}
