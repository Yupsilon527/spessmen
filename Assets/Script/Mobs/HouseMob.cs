using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CompartimentComponent))]
public class HouseMob : BuildingMob
{
    CompartimentComponent indoors;
    protected override void Awake()
    {
        base.Awake();
        indoors = GetComponent<CompartimentComponent>();
    }
    public override void SetBuildPercentage(float percent)
    {
        base.SetBuildPercentage(percent);
        if (indoors!=null && indoors.entryDoor!=null)
        indoors.entryDoor.gameObject.SetActive(percent >= 100);
    }
}
