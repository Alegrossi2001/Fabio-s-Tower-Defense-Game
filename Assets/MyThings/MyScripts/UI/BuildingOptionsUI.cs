using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingOptionsUI : MonoBehaviour
{
    [SerializeField] private Button buildingOptionsButton;

    public void SetBuildingOptionsOn()
    {
        BuildingOptions.buildingOptionsActive = !BuildingOptions.buildingOptionsActive;
        if(BuildingOptions.buildingOptionsActive)
        {
            buildingOptionsButton.image.color= Color.green;
        }
        else
        {
            buildingOptionsButton.image.color= Color.red;
        }
    }
}
