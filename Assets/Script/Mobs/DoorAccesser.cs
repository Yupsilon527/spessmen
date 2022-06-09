using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorAccesser : MonoBehaviour
{
    public Player parent;
    public CompartimentComponent HouseInRange;
    private void Update()
    {
        if (Input.GetButtonDown("Enter"))
        {
            if (parent.movement.IsInside())
            {
                ExitHouse();
            }
            else if (HouseInRange != null)
            {
                EnterHouse();
            }
        }
        if (Input.GetButtonDown("Dig"))
        {
            if (parent.movement.IsInside())
            {
                parent.backpack.TransferItem( parent.movement.indoor.GetComponent<InventoryComponent>());
            }
        }
    }
    void EnterHouse()
    {
        parent.menu.OpenIndoorsMenu((HouseMob)HouseInRange.Owner);
        HouseInRange.LoadMob(parent.movement);
    }
    void ExitHouse()
    {
        parent.menu.CloseMenu();
        parent.movement.ExitBuilding();
    }
}
