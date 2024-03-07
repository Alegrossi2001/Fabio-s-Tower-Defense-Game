using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building
{
    public BuildingHealth healthManager { get; private set; }
    public Transform buildingPosition { get; private set; }
    public static EventHandler<OnBuildingActionEventArgs> OnBuildingAction;
    private bool isHQ;
    private float shootingDistance;
    public Vector3 currentTarget;
    private List<Vector3> targets = new List<Vector3>();
    private Enemy currentEnemyTarget;

    public Building(GameObject building, BuildingSO buildingSO, bool isHQ=false)
    {
        healthManager = new BuildingHealth(this, building, buildingSO.health);
        this.buildingPosition = building.transform;
        this.isHQ = isHQ;
        this.shootingDistance = buildingSO.shootingDistance;
        building.GetComponent<BuildingCollisionHandler>().AssignBuilding(this);
        InitialiseBuilding();
        
    }

    private void InitialiseBuilding()
    {
        OnBuildingAction?.Invoke(this, new OnBuildingActionEventArgs
        {
            building = this,
            buildingPosition = this.buildingPosition,
            isHQ = this.isHQ
        });
    }

    public bool CalculateClosestTarget()
    {
        if(targets.Count > 0)
        {
            foreach(Vector3 target in targets)
            {
                if(target == null)
                {
                    targets.Remove(target);
                }
            }
            Vector3 potentialTarget = Utilities.GetClosestObject(targets, buildingPosition.position);
            this.currentTarget = potentialTarget;
            if(Vector3.Distance(this.currentTarget, this.buildingPosition.position) <= this.shootingDistance)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    public void ShootBulletAtClosestEnemy(Enemy enemy)
    {
        Vector3 offset = new Vector3(0, 1.75f, 0);
        Vector3 shootingPosition = buildingPosition.position + offset;
        Bullet bullet = new Bullet(shootingPosition, enemy);
    }

    public void SetTargets(List<Vector3> targets)
    {
        this.targets = targets;
    }

    public Enemy GetClosestEnemyTarget(List<Enemy> enemies)
    {
        return Utilities.GetClosestObject(enemies, buildingPosition.position);
    }


}
