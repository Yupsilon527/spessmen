using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorAccesser : MobComponent
{
    public CompartimentComponent HouseInRange;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (Owner.IsInside())
            {
                ExitHouse();
            }
            else if (HouseInRange != null)
            {
                EnterHouse();
            }
        }
    }
    void EnterHouse()
    {
        PlayerMob player = (PlayerMob)Owner;
        player.parent.menu.OpenIndoorsMenu((HouseMob)HouseInRange.Owner);
        HouseInRange.LoadMob(player);
    }
    void ExitHouse()
    {
        PlayerMob player = (PlayerMob)Owner;
        player.parent.menu.CloseMenu();
        player.ExitBuilding();
    }
}
