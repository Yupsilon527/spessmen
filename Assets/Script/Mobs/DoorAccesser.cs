using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorAccesser : MobComponent
{
    CompartimentComponent HouseInRange;
    public void OnEnterInRange(CompartimentComponent trigger)
    {
        if (HouseInRange != trigger)
        {
            HouseInRange = trigger;
        }
    }
    public void OnExitInRange(CompartimentComponent trigger)
    {
        if (HouseInRange == trigger)
        {
            HouseInRange = null;
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (Owner.IsInside())
            {
                ((PlayerMob)Owner).ExitBuilding();
            }
            else if (HouseInRange != null)
            {
                HouseInRange.LoadMob((PlayerMob)Owner);
            }
        }
    }
}
