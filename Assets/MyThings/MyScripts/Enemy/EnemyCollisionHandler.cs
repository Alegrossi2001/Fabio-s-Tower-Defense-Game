using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCollisionHandler : MonoBehaviour
{
    private Enemy enemyAssigned;

    public void AssignEnemy(Enemy enemy)
    {
        enemyAssigned = enemy;
    }

    private void OnTriggerEnter(Collider collision)
    {
        //Collide with building
        if(collision.gameObject.GetComponent<BuildingCollisionHandler>() != null)
        {
            BuildingCollisionHandler buildingcollided = collision.gameObject.GetComponent<BuildingCollisionHandler>();
            buildingcollided.RemoveHealth(50);
            enemyAssigned.EnemyIsDead();
        }
    }
}
