using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorAccesser : PlayerComponent
{
    public CompartimentComponent HouseInRange;
    private void Update()
    {
        if (Input.GetButtonDown("Build/Enter"))
        {
            if (parent.IsInside())
            {
                /*if (parent.indoor.parentMob is HouseMob house && (house.drive?.IsDriveMode() ?? false))
                {
                    parent.menu.OpenIndoorsMenu(house);
                    house.drive.ExitDriveMode();
                }
                else*/
                {
                    ExitHouse();
                }
            }
            else if (HouseInRange != null)
            {
                EnterHouse();
            }
        }
        if (Input.GetButtonDown("Dig"))
        {
            if (parent.IsInside())
            {
                parent.backpack.TransferItem(parent.indoor.GetComponent<InventoryComponent>());
            }
        }
    }
    void EnterHouse()
    {

        //SFX Door sound
        parent.menu.OpenIndoorsMenu((HouseMob)HouseInRange.parentMob);
        HouseInRange.LoadMob(parent);
        AudioManager.Instance.PlaySfx("Door Open", 3);
    }
    void ExitHouse()
    {
        //SFX Door sound
        AudioManager.Instance.PlaySfx("Door Close", 4);
        parent.menu.CloseMenu();
        parent.ExitBuilding();
    }
}
