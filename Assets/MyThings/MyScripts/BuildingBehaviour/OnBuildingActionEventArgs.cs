using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnBuildingActionEventArgs : EventArgs
{
    public Building building;
    public Transform buildingPosition;
    public bool? isHQ;
}
