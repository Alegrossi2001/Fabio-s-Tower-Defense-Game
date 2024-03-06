using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstructionManager : MonoBehaviour
{
    private BuildingListSO buildingList;
    private BuildingSO activeBuilding;
    private List<Building> buildingsInScene = new List<Building>();

    private void Awake()
    {
        this.buildingList = Resources.Load<BuildingListSO>(typeof(BuildingListSO).Name);
        InitialiseHQ();
        
    }

    void Start()
    {
        OnMouseValidator.onTouch += TriggerBuildingConstruction;
    }

    private void InitialiseHQ()
    {
        //hardcoded value, change as necessary
        activeBuilding = buildingList.buildingList[1];
        Vector3 offset = new Vector3(0, 0, 0);
        Vector3 positionToBuild = new Vector3(0,2,2) + offset;
        GameObject building = Instantiate(activeBuilding.buildingPrefab, positionToBuild, Quaternion.identity);
        building.layer = 8;
        Building buildingCreated = new Building(building, activeBuilding, true);
        buildingsInScene.Add(buildingCreated);
    }

    private void TriggerBuildingConstruction(object sender, OnTouchEventArgs e)
    {
        //Hardcoded Value, change when more buildings are loaded
        activeBuilding = buildingList.buildingList[0];
        Vector3 offset = new Vector3(0, 1.25f, 0);
        Vector3 positionToBuild = e.collider.transform.position + offset;
        GameObject building = Instantiate(activeBuilding.buildingPrefab, positionToBuild, Quaternion.identity);
        building.layer = 8;
        Building buildingCreated = new Building(building, activeBuilding);
        buildingsInScene.Add(buildingCreated);
    }
}
