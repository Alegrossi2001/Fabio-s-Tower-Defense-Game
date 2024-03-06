using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/EnemyListSO")]
public class EnemyListSO : ScriptableObject
{
    public List<EnemySO> EnemyTypes;
}
