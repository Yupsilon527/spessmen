using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CompartimentComponent))]
[RequireComponent(typeof(InventoryComponent))]
public class HouseMob : BuildingMob
{
    public CompartimentComponent indoors;
    public InventoryComponent storage;
    protected override void Awake()
    {
        base.Awake();
        indoors = GetComponent<CompartimentComponent>();
        storage = GetComponent<InventoryComponent>();
    }
    public override void SetBuildPercentage(float percent)
    {
        base.SetBuildPercentage(percent);
        if (indoors != null && indoors.entryDoor != null)
        {
            indoors.entryDoor.gameObject.SetActive(percent >= 100);
        }
    }
}
