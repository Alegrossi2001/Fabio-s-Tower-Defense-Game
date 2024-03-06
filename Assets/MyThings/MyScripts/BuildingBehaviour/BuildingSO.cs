using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Buildings/Building")]
public class BuildingSO : ScriptableObject
{
    public string nameString;
    public int health;
    public GameObject buildingPrefab;
    public float shootingDistance;
}
