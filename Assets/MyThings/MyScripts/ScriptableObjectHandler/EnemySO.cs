using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Enemy/EnemySO")]
public class EnemySO : ScriptableObject
{
    public string nameString;
    public int health;
    public GameObject enemyPrefab;
}
